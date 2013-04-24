using AEGIScript.Lang;
using AEGIScript.Lang.Evaluation;
using AEGIScript.Lang.FunCalls;
using AEGIScript.Lang.Scope;
using AEGIScript.Lang.SymbolTables;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AEGIScript.GUI.Model
{
    class Interpreter
    {
        private FunCallHelper helper = new FunCallHelper();
        private Random rand = new Random();
        private Scope GlobalScope = new Scope();
        private Stack<SymbolTable> Scopes = new Stack<SymbolTable>();
        private StringBuilder OutputBuilder = new StringBuilder();
        private DateTime BeginTime = new DateTime();
        private ANTLRStringStream sStream { get; set; }
        private aegiscriptLexer lexer { get; set; }
        private CommonTokenStream tokens { get; set; }
        private aegiscriptParser parser { get; set; }
        public event EventHandler<PrintEventArgs> Print;

        public Interpreter() 
        {
            ScopeMediator.newScope += ScopeMediator_newScope;
            ScopeMediator.removeScope += ScopeMediator_removeScope;
        }

        // far from final implementation
        private void ScopeMediator_removeScope(object sender, EventArgs e)
        {
            if (Scopes.Count != 0)
            {
                Scopes.Pop();
            }
        }

        // not final implementation
        private void ScopeMediator_newScope(object sender, EventArgs e)
        {
            Scopes.Push(new SymbolTable());
        }

        /// <summary>
        /// Runs the source file through the lexer and the parser and then returns the AST representation.
        /// </summary>
        /// <param name="source">Source file to be interpreted</param>
        /// <returns>AST representation of source file</returns>
        public string GetAstAsString(string source)
        {
            string output;
            SetParser(source);
            try
            {
                output = parser.program().Tree.ToStringTree();
                
            } // check for parsing errors
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }


        /// <summary>
        /// Interpret presented source file
        /// </summary>
        /// <param name="source">source to be interpreted</param>
        /// <returns>Program output</returns>
        public string Interpret(string source)
        {
            string output;
            SetParser(source);
            try
            {
                //CommonTree root = parser.program().Tree.GetChild(0) as CommonTree;
                //output = root.Token.ToString() + " " + root.Text;
                output = PrettyPrinting(parser.program().Tree);
            } // check for parsing errors
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }


        



        /// <summary>
        /// Provides a layer of abstraction for walking ASTNodes -- handles double dispatching gracefully
        /// </summary>
        /// <param name="node"></param>
        /// <returns>String representation for debug purposes</returns>
        private string Walk(ASTNode node)
        {
            switch (node.ActualType)
            {
                case ASTNode.Type.ASSIGN:
                    return Walk(node as AssignNode);
                case ASTNode.Type.WHILE:
                    return Walk(node as WhileNode);
                case ASTNode.Type.IF:
                    return Walk(node as IfNode);
                case ASTNode.Type.ELIF:
                    return Walk(node as ElsifNode);
                case ASTNode.Type.ELSE:
                    return Walk(node as ElseNode);
                case ASTNode.Type.FUNCALL:
                    return Walk(node as FunCallNode);
                default:
                    throw new Exception("RUNTIME ERROR! \n Invalid statement on line: " + node.Line);
            }
        }

        
        /// <summary>
        /// Interface provided for the ViewModel for UI actions
        /// </summary>
        /// <param name="source">Source code to interpret</param>
        /// <returns>Interpreter output</returns>
        public string Walk(string source, bool FromImmediate = false)
        {
            if (!FromImmediate)
            {
                GlobalScope.Clear();
            }
            SetParser(source);

            var ret = parser.program();

            // can't really determine where the problem came from
            // ANTLR suppresses errors during parsing and lexing
            if (lexer.NumberOfSyntaxErrors > 0)
	        {
                Print(this, new PrintEventArgs("Lexical error!"));
                return "Lexical error!";
	        }
            else if (parser.NumberOfSyntaxErrors > 0)
            {
                Print(this, new PrintEventArgs("Syntax error!"));
                return "Syntax error!";
            }
            return Walk(ret.Tree as CommonTree);
        }

        /// <summary>
        /// The soul of the interpreter -- walks the AST and interprets it
        /// </summary>
        /// <param name="node">Root of the AST</param>
        private string Walk(BeginNode node)
        {
            BeginTime = DateTime.Now;

            StringBuilder builder = new StringBuilder();

            foreach (ASTNode n in node.Children)
            {
                try
                {
                    builder.Append(Walk(n));
                }
                catch (Exception ex)
                {
                    // crashes here if the exception raised is not a built-in type
                    builder.AppendLine(ex.Message);
                    PrintFun(ex.Message);
                    return builder.ToString();
                }
            }
            TimeSpan ElapsedTime = DateTime.Now - BeginTime;
            var AsDouble = Math.Truncate(ElapsedTime.TotalSeconds * 10000) / 10000;
            Print(this, new PrintEventArgs("Successfully finished in: " + AsDouble.ToString(CultureInfo.InvariantCulture) + " second(s)."));
            return builder.ToString();
        }
        private string Walk(CommonTree tree)
        {
            try
            {
                BeginNode begin = new BeginNode(tree as CommonTree);
                return Walk(begin);
            }
            catch (Exception ex)
            {
                PrintFun(ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// Performs basic assignment -- right now for globalscope only
        /// </summary>
        /// <param name="node">An AssignNode</param>
        /// <returns>Assigned variable as string</returns>
        private string Walk(AssignNode node)
        {
            TermNode Resolved = Resolve(node.Children[1], ASTNode.Type.BOOL);
            if (node.Children[0].ActualType == ASTNode.Type.ARRACC)
            {
                var Accessor = node.Children[0] as ArrAccessNode;
                if (GlobalScope.GetVar(Accessor.Symbol).ActualType != ASTNode.Type.ARRAY)
                {
                    throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                        "non-array object, or using an out-of-range index at line " + node.Line + "\n");
                }
                var ActualArray = GlobalScope.GetVar(Accessor.Symbol) as ArrayNode;
                var Orig = ActualArray;
                TermNode ResolvedInd;
                for (int i = 1; i < Accessor.Children.Count - 1; i++)
                {
                    ResolvedInd = Resolve(Accessor.Children[i], ASTNode.Type.BOOL);
                    if (ResolvedInd.ActualType != ASTNode.Type.INT)
                    {
                        throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " + node.Line + "\n");
                    }
                    int Ind = (ResolvedInd as IntNode).Value;
                    if (Ind >= 0 && Ind < ActualArray.Elements.Count
                        && ActualArray[Ind].ActualType == ASTNode.Type.ARRAY)
                    {
                        ActualArray = ActualArray[Ind] as ArrayNode;
                    }
                    else
                    {
                        throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                        "non-array object, or using an out-of-range index at line " + node.Line + "\n");
                    }
                }
                ResolvedInd = Resolve(Accessor.Children[Accessor.Children.Count - 1], ASTNode.Type.BOOL);
                if (ResolvedInd.ActualType != ASTNode.Type.INT)
                {
                    throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " + node.Line + "\n");
                }
                int finalInd = (ResolvedInd as IntNode).Value;
                ActualArray[finalInd] = Resolved;
                GlobalScope.AddVar(Accessor.Symbol, Orig);
            }
            else 
                GlobalScope.AddVar((node.Children[0] as VarNode).Symbol, Resolved);
            return Resolved.ToString() + " " + Resolved.ActualType.ToString() + "\n";
        }


        /// <summary>
        /// Walks an elsif clause
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Clause result as string</returns>
        private string Walk(ElsifNode node)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var elif in node.Children)
            {
                if (elif != node.Children[0])
                {
                    builder.Append(Walk(elif));
                }
            }
            return builder.ToString();
        }

        

        /// <summary>
        /// Walks a whole 'if' statement
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Statement result as string</returns>
        private string Walk(IfNode node)
        {
            BooleanNode cond = Resolve(node.Children[0], ASTNode.Type.BOOL) as BooleanNode;
            StringBuilder builder = new StringBuilder();
            if (cond.Value)
            {
                GlobalScope.NewScope();
                foreach (var n in node.Children)
                {
                    if (!(n is ElsifNode) && !(n is ElseNode) && n != node.Children[0]) // we skip the else and elif clauses
                    {
                        builder.Append(Walk(n));
                    }
                }
                GlobalScope.RemoveScope();
            }
            else if(node.Clauses.Count > 0) // if the if statement has elif clauses
            {
                GlobalScope.NewScope();
                foreach (var cl in node.Clauses)
                {
                    cond = Resolve(cl.Children[0], ASTNode.Type.BOOL) as BooleanNode;
                    if (cond.Value)
                    {
                        builder.Append(Walk(cl));
                        break; // we only want to walk a single elif clause
                    }
                }
                GlobalScope.RemoveScope();
            }
            else if(node.Else != null)
            {
                builder.Append(Walk(node.Else));
            }
            return builder.ToString();
        }
        /// <summary>
        /// Walks the else part of an 'if' statement
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Clause result as string</returns>
        private string Walk(ElseNode node)
        {
            GlobalScope.NewScope();
            StringBuilder builder = new StringBuilder();
            foreach (var n in node.Children)
            {
                if (n != node.Children[0])
                {
                    builder.Append(Walk(n));
                }
            }
            GlobalScope.RemoveScope();
            return builder.ToString();
        }

        /// <summary>
        /// Provides a method to call functions with parameters provided by the interpreter
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string Walk(FunCallNode node)
        {
            if (node.FunName == "print" && node.Children.Count == 2)
            {
                var Resolved = Resolve(node.Children[1], ASTNode.Type.BOOL);
                PrintFun(Resolved);
                // should return something meaningful
                return Resolved.ToString();
            }
            else if (helper.Contains(node.FunName))
            {
                return "";
            }
            else
            {
                StringBuilder b = new StringBuilder();
                for (int i = 1; i < node.Children.Count; i++)
                {
                    b.Append(node.Children[i].ToString() + " ");
                }
                throw new Exception("RUNTIME ERROR! \n Undefined function call to: " + node.FunName + "\n" + 
                                    "with args: " + b.ToString());
            }
        }

        private void PrintFun(TermNode node)
        {
            Print(this, new PrintEventArgs(node));
        }

        private void PrintFun(String message)
        {
            Print(this, new PrintEventArgs(message));
        }

        /// <summary>
        /// Walks a 'while' node statement
        /// </summary>
        /// <param name="node">While node</param>
        /// <returns>Whole result of the loop</returns>
        private string Walk(WhileNode node)
        {
            StringBuilder builder = new StringBuilder();
            BooleanNode cond = Resolve(node.Children[0], ASTNode.Type.BOOL) as BooleanNode;
            while (cond.Value)
            {
                GlobalScope.NewScope();
                for (int i = 1; i < node.Children.Count; i++) // we skip the condition
			    {
			        builder.Append(Walk(node.Children[i]));
			    }
                GlobalScope.RemoveScope();
                cond = Resolve(node.Children[0], ASTNode.Type.BOOL) as BooleanNode; // resolve it every time
            }
            return builder.ToString();
        }

        /// <summary>
        /// Trivial resolve 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private TermNode Resolve(TermNode node)
        {
            return node;
        }

        /// <summary>
        /// Resolves an arithmetic node recursively
        /// </summary>
        /// <param name="node">Node to be resolved</param>
        /// <param name="currentType">Current strongest type</param>
        /// <returns>Type of the arithmetic expression</returns>
        /// TODO: eliminate second parameter
        private TermNode Resolve(ArithmeticNode toRes, ASTNode.Type currentType)
        {
            TermNode left, right;
            left = Resolve(toRes.Children[0], currentType);
            right = Resolve(toRes.Children[1], currentType);
            return NodeArithmetics.Op(left, right, toRes.op);
        }

        private TermNode Resolve(FunCallNode fun, ASTNode.Type currentType)
        {
            List<TermNode> ResolvedArgs = new List<TermNode>();
            for (int i = 1; i < fun.Children.Count; i++)
            {
                ResolvedArgs.Add(Resolve(fun.Children[i], currentType));
            }
            switch (fun.FunName)
            {
                case "rand":
                    Random rand = new Random();
                    if (fun.Children.Count == 1)
                    {
                        return new IntNode(rand.Next());
                    }
                    else if(fun.Children.Count == 2)
                    {
                        if (ResolvedArgs[0].ActualType == ASTNode.Type.INT)
                        {
                            return new IntNode(rand.Next((ResolvedArgs[0] as IntNode).Value));
                        }
                        else
	                    {
                            throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line + "\n");
	                    }
                    }
                    else
	                {
                        throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line + "\n");
	                }
                default:
                    throw new Exception("RUNTIME ERROR!\n You are either calling a non-existing function, or trying to "+ 
                                        "use a void functions return value.");
            }
        }

        /// <summary>
        /// Provides an abstraction layer for resolves -- handles double dispatching to the proper functions
        /// </summary>
        /// <param name="toRes"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        private TermNode Resolve(ASTNode toRes, ASTNode.Type currentType)
        {
            switch (toRes.ActualType)
            {
                case ASTNode.Type.ARITH:
                    return Resolve(toRes as ArithmeticNode, currentType);
                case ASTNode.Type.INTVAR: case ASTNode.Type.BOOLVAR: case ASTNode.Type.DOUBLEVAR: case ASTNode.Type.STRINGVAR: case ASTNode.Type.VAR:
                    return Resolve(toRes as VarNode, currentType);
                case ASTNode.Type.INT: case ASTNode.Type.STRING: case ASTNode.Type.BOOL: case ASTNode.Type.DOUBLE:
                    return Resolve(toRes as TermNode, currentType);
                case ASTNode.Type.ARRAY:
                    return Resolve(toRes as ArrayNode, currentType);
                case ASTNode.Type.ASSIGN:
                    TermNode Resolved = Resolve(toRes.Children[1], ASTNode.Type.BOOL);
                    GlobalScope.AddVar((toRes.Children[0] as VarNode).Symbol, Resolved);
                    return Resolved;
                case ASTNode.Type.ARRACC:
                    return Resolve(toRes as ArrAccessNode, currentType);
                case ASTNode.Type.FUNCALL:
                    return Resolve(toRes as FunCallNode, currentType);
                default:
                    throw new Exception("Unable to resolve ASTNode " + toRes.ActualType.ToString() + " " + toRes.Parent.ActualType.ToString());
            }
        }

        /// <summary>
        /// Resolves Array Accessor. The main idea is that apart from the last index, all indices
        /// need to resolve to arrays, so we iterate through them, and then we resolve the last element
        /// </summary>
        /// <param name="toRes"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        private TermNode Resolve(ArrAccessNode toRes, ASTNode.Type currentType)
        {
            var Ret = GlobalScope.GetVar(toRes.Symbol);
            if (Ret.ActualType != ASTNode.Type.ARRAY)
            {
                throw new Exception("RUNTIME ERROR!\n You are trying to use an accessor on a non-array object at line " + toRes.Line + "\n");
            }
            ArrayNode ActualArray = Ret as ArrayNode;
            TermNode ResolvedInd;
            for (int i = 1; i < toRes.Children.Count - 1; i++)
            {
                ResolvedInd = Resolve(toRes.Children[i], currentType);
                if (ResolvedInd.ActualType != ASTNode.Type.INT)
                {
                    throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " + toRes.Line + "\n");
                }
                int Ind = (ResolvedInd as IntNode).Value;
                if (Ind >= 0 && Ind < ActualArray.Elements.Count
                    && ActualArray[Ind].ActualType == ASTNode.Type.ARRAY)
                {
                    ActualArray = ActualArray[Ind] as ArrayNode;
                }
                else
                {
                    throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                    "non-array object, or using an out-of-range index at line " + toRes.Line + "\n");
                }
            }
            ResolvedInd = Resolve(toRes.Children[toRes.Children.Count - 1], currentType);
            if (ResolvedInd.ActualType != ASTNode.Type.INT)
            {
                throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " + toRes.Line + "\n");
            }
            return Resolve(ActualArray[(ResolvedInd as IntNode).Value], currentType);
        }

        /// <summary>
        /// Resolves array nodes
        /// </summary>
        /// <param name="toRes">An Array node</param>
        /// <param name="currentType"></param>
        /// <returns>Node with elements as terms</returns>
        private TermNode Resolve(ArrayNode toRes, ASTNode.Type currentType)
        {
            foreach (var Node in toRes.Children)
            {
                toRes.Elements.Add(Resolve(Node, currentType));
            }
            return toRes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toRes"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        private TermNode Resolve(AssignNode toRes, ASTNode.Type currentType)
        {
            return Resolve(toRes.Children[1], currentType);
        }

        private TermNode Resolve(TermNode toRes, ASTNode.Type currentType)
        {
            return toRes;
        }

        private TermNode Resolve(VarNode toRes, ASTNode.Type currentType)
        {
            TermNode node;
            switch (toRes.ActualType)
	        {
                case ASTNode.Type.INTVAR:
                    node = (toRes as IntVarNode).Interpret(GlobalScope);
                    if (ASTNode.Type.INT > currentType)
                    {
                        currentType = ASTNode.Type.INT;
                    }
                    return node;
                case ASTNode.Type.BOOLVAR:
                    node = (toRes as BoolVarNode).Interpret(GlobalScope);
                    if (ASTNode.Type.BOOL > currentType)
                    {
                        currentType = ASTNode.Type.BOOL;
                    }
                    return node;
                case ASTNode.Type.DOUBLEVAR:
                    node = (toRes as DoubleVarNode).Interpret(GlobalScope);
                    if (ASTNode.Type.DOUBLE > currentType)
                    {
                        currentType = ASTNode.Type.DOUBLE;
                    }
                    return node;
                case ASTNode.Type.STRINGVAR:
                    node = (toRes as StringVarNode).Interpret(GlobalScope);
                    if (ASTNode.Type.STRING > currentType)
                    {
                        currentType = ASTNode.Type.STRING;
                    }
                    return node;
                case ASTNode.Type.VAR:
                    return GlobalScope.GetVar(toRes.Symbol);
                case ASTNode.Type.ARRAY:
                    return GlobalScope.GetVar(toRes.Symbol);
                default:
                    return toRes;
	        }
        }

        private int IndexOfChild(ASTNode node)
        {
            for (int i = 0; i < node.Parent.Children.Count; i++)
            {
                if (node.Parent.Children[i] == node)
                {
                    return i;
                }
            }
            throw new Exception("Parent uninitialized");
        }




        private int TreeDepth = 0;
        /// <summary>
        /// Calculates the depth of the current AST.
        /// </summary>
        /// <param name="tree">AST root</param>
        /// <param name="curDep">Recursive depth counter</param>
        private void SetTreeDepth(CommonTree tree, int curDep = 0)
        {
            if (curDep > TreeDepth)
            {
                TreeDepth = curDep;
            }
            for (int i = 0; i < tree.ChildCount; i++)
            {
                SetTreeDepth(tree.GetChild(i) as CommonTree, curDep + 1);
            }
        }

        /// <summary>
        /// Sets up the parser
        /// </summary>
        /// <param name="source">Path to source file</param>
        private void SetParser(string source)
        {
            sStream = new ANTLRStringStream(source);
            lexer = new aegiscriptLexer(sStream);
            tokens = new CommonTokenStream(lexer);
            parser = new aegiscriptParser(tokens);

            TokenTypeMediator.SetTokens(parser.TokenNames);
        }

        public string GetASTTokensAsString(string source)
        {
            SetParser(source);
            return PrettyPrinting(parser.program().Tree, true, true);
        }



        public string GetASTObjectsAsString(string source)
        {
            SetParser(source);
            return PrettyPrinting(parser.program().Tree, false, true);
        }

        /// <summary>
        /// Helper class for depth-first descent
        /// </summary>
        class CommonTreeWrapper
        {
            public CommonTreeWrapper(CommonTree ct)
            {
                Tree = ct;
            }

            public bool Visited { get; set; }
            public int End { get; set; }
            public int Depth { get; set; }
            public CommonTree Tree { get; private set; }
        }

        private string PrettyPrinting(CommonTree tree, bool TokenTextOnly = false, bool PrintTypes = false)
        {
            StringBuilder Builder = new StringBuilder();
            if (PrintTypes)
            {
                TreeDepth = 0;
                SetTreeDepth(tree, 0);
            }
            PreOrder(tree, ref Builder, -1, TokenTextOnly, PrintTypes); // reason for depth == -1 is that ANTLR builds the tree with the root as null
            return Builder.ToString();
        }

        /// <summary>
        /// Does a pre-order walk of the ast to print out different nodes for debugging reasons
        /// </summary>
        /// <param name="node">Current root</param>
        /// <param name="builder">A StringBuilder for prettyprinting</param>
        /// <param name="depth">Current Depth</param>
        /// <param name="TokenTextOnly">PrintFun only the content contained in the token</param>
        /// <param name="PrintTypes">PrintFun out the token's type</param>
        private void PreOrder(CommonTree node, ref StringBuilder builder, int depth, bool TokenTextOnly = false, bool PrintTypes = false)
        {
            AddIndentation(ref builder, depth);
            if (node.Token != null)
            {
                int TokenLengthFactor = node.Token.Text.Length / 4;
                int OffsetFromMax = 2;
                if (TokenTextOnly)
                {
                    builder.Append(node.Token.Text);
                    TokenLengthFactor = node.Token.Text.Length / 4;
                }
                else
                {
                    builder.Append(node.Token.ToString());
                    TokenLengthFactor = node.Token.ToString().Length / 4;
                    OffsetFromMax += 4;
                }
                if (PrintTypes && node.Type >= 0 && node.Type < parser.TokenNames.Length)
                {
                    AddIndentation(ref builder, (TreeDepth + OffsetFromMax - TokenLengthFactor - depth));
                    builder.Append(parser.TokenNames[node.Type]);
                    ASTNode Node = new ASTNode(node, parser.TokenNames);
                }
                builder.Append('\n');
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                PreOrder(node.GetChild(i) as CommonTree, ref builder, depth + 1, TokenTextOnly, PrintTypes);
            }
        }

        /// <summary>
        /// PrettyPrinting helper, adds depth to the StringBuilder. Not conventional, but for deep trees, this is faster than the standard recursive call.
        /// </summary>
        /// <param name="builder">A stringbuilder</param>
        /// <param name="depth">Current depth</param>
        private void AddIndentation(ref StringBuilder builder, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                builder.Append('\t');
            }
        }

        private Dictionary<CommonTree, DFS_Helper> VisitHelper = new Dictionary<CommonTree, DFS_Helper>();

        private class DFS_Helper
        {
            public DFS_Helper(int depth, bool visited)
            {
                Depth = depth;
                Visited = visited;
            }

            public int Depth { get; private set; }
            public int Order { get; set; }
            public int End { get; set; }
            public bool Visited { get; private set; }

        }

        /// <summary>
        /// A standard DFS, for visiting and prettyprinting purposes
        /// </summary>
        /// <param name="node">Current root</param>
        /// <param name="depth">Current depth</param>
        /// <param name="v_current">Count of visited nodes</param>
        /// <param name="e_current">Count of "ended" nodes</param>
        private void DepthFirstDescent(CommonTree node, int depth, ref int v_current, ref int e_current)
        {
            if (!VisitHelper.ContainsKey(node))
            {
                VisitHelper.Add(node, new DFS_Helper(depth, true));
                VisitHelper[node].Order = v_current++;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                if (!VisitHelper.ContainsKey(node))
                {
                    DepthFirstDescent(node, depth + 1, ref v_current, ref e_current);
                }
            }
            VisitHelper[node].End = e_current++;
        }


        private void DepthFirstDescent(CommonTree node, int depth, ref StringBuilder builder, ref int v_current, ref int e_current)
        {
            if (!VisitHelper.ContainsKey(node))
            {
                VisitHelper.Add(node, new DFS_Helper(depth, true));
                VisitHelper[node].Order = v_current++;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                if (!VisitHelper.ContainsKey(node.GetChild(i) as CommonTree))
                {
                    DepthFirstDescent(node.GetChild(i) as CommonTree, depth + 1, ref builder, ref v_current, ref e_current);
                }
            }
            VisitHelper[node].End = e_current++;
            if (node.Token != null)
            {
                AddIndentation(ref builder, depth);
                builder.Append(node.Token.Text + "\t\t Depth : " + VisitHelper[node].Depth +  "\n");
            }
        }

        public string PrintAST_DFS(string source)
        {
            SetParser(source);
            //SetTreeDepth(parser.program().Tree);
            int depth = 0;
            int end = 0;
            StringBuilder builder = new StringBuilder();
            DepthFirstDescent(parser.program().Tree, 0, ref builder, ref depth, ref end);
            return builder.ToString();
        }
    }
}
