using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AEGIScript.Lang;
using AEGIScript.Lang.Evaluation;
using AEGIScript.Lang.FunCalls;
using AEGIScript.Lang.Scoping;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace AEGIScript.GUI.Model
{
    internal class Interpreter
    {
        private readonly FunCallHelper _helper = new FunCallHelper();
        private readonly Random _rand = new Random();
        private readonly Scope _scope = new Scope();
        private readonly Dictionary<CommonTree, DfsHelper> _visitHelper = new Dictionary<CommonTree, DfsHelper>();
        private DateTime _beginTime;
        private int _treeDepth;

        /// <summary>
        /// Fields provided by ANTLR
        /// </summary>
        private ANTLRStringStream SStream { get; set; }
        private aegiscriptLexer Lexer { get; set; }
        private CommonTokenStream Tokens { get; set; }
        private aegiscriptParser Parser { get; set; }
        public event EventHandler<PrintEventArgs> Print;
        public StringBuilder Output { get; private set; }

        public Interpreter()
        {
            Output = new StringBuilder();
        }

        /// <summary>
        ///     Runs the source file through the lexer and the parser and then returns the AST representation.
        /// </summary>
        /// <param name="source">Source file to be interpreted</param>
        /// <returns>AST representation of source file</returns>
        public string GetAstAsString(string source)
        {
            string output;
            SetParser(source);
            try
            {
                output = Parser.program().Tree.ToStringTree();
            } // check for parsing errors
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }


        /// <summary>
        ///     Interpret presented source file
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
                output = PrettyPrinting(Parser.program().Tree);
            } // check for parsing errors
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }


        /// <summary>
        ///     Provides a layer of abstraction for walking ASTNodes -- handles double dispatching gracefully
        ///     From a grammatical point of view, walk handle actual statements
        /// </summary>
        /// <param name="node"></param>
        /// <returns>String representation for debug purposes</returns>
        /// TODO: 
        ///     - Remove string as a return value, should increase performance
        ///       by a big margin
        private void Walk(ASTNode node)
        {
            switch (node.ActualType)
            {
                case ASTNode.Type.ASSIGN:
                    Walk(node as AssignNode);
                    break;
                case ASTNode.Type.WHILE:
                    Walk(node as WhileNode);
                    break;
                case ASTNode.Type.IF:
                    Walk(node as IfNode);
                    break;
                case ASTNode.Type.ELIF:
                    Walk(node as ElsifNode);
                    break;
                case ASTNode.Type.ELSE:
                    Walk(node as ElseNode);
                    break;
                case ASTNode.Type.FUNCALL:
                    Walk(node as FunCallNode);
                    break;
                default:
                    throw new Exception("RUNTIME ERROR! \n Invalid statement on line: " + node.Line);
            }
        }


        /// <summary>
        ///     Interface provided for the ViewModel for UI actions
        /// </summary>
        /// <param name="source">Source code to interpret</param>
        /// <param name="fromImmediate">True, if we need to interpret immediate code</param>
        /// <returns>Interpreter output</returns>
        public void Walk(string source, bool fromImmediate = false)
        {
            Output.Clear();
            if (!fromImmediate)
            {
                _scope.Clear();
            }
            SetParser(source);

            AstParserRuleReturnScope<CommonTree, IToken> ret = Parser.program();

            // can't really determine where the problem came from
            // ANTLR suppresses errors during parsing and lexing
            if (Lexer.NumberOfSyntaxErrors > 0)
            {
                //Print(this, new PrintEventArgs("Lexical error!"));
                Output.Append("Lexical error!");
            }
            if (Parser.NumberOfSyntaxErrors > 0)
            {
                //Print(this, new PrintEventArgs("Syntax error!"));
                Output.Append("Syntax error!");
            }
            Walk(ret.Tree);
        }

        /// <summary>
        ///     The soul of the interpreter -- walks the AST and interprets it
        /// </summary>
        /// <param name="node">Root of the AST</param>
        private void Walk(BeginNode node)
        {
            _beginTime = DateTime.Now;


            foreach (ASTNode n in node.Children)
            {
                try
                {
                    Walk(n);
                }
                catch (Exception ex)
                {
                    // crashes here if the exception raised is not a built-in type
                    PrintFun(ex.Message);
                }
            }
            TimeSpan elapsedTime = DateTime.Now - _beginTime;
            double asDouble = Math.Truncate(elapsedTime.TotalSeconds*10000)/10000;
            Output.Append("Successfully finished in: " + asDouble.ToString(CultureInfo.InvariantCulture) +
                          " second(s).");
            /*Print(this,
                  new PrintEventArgsx("Successfully finished in: " + asDouble.ToString(CultureInfo.InvariantCulture) +
                                     " second(s)."));*/
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private void Walk(CommonTree tree)
        {
            try
            {
                var begin = new BeginNode(tree);
                Walk(begin);
            }
            catch (Exception ex)
            {
                PrintFun(ex.Message);
            }
        }

        /// <summary>
        ///     Performs basic assignment -- right now for globalscope only
        /// </summary>
        /// <param name="node">An AssignNode</param>
        /// <returns>Assigned variable as string</returns>
        private void Walk(AssignNode node)
        {
            TermNode resolved = Resolve(node.Children[1], ASTNode.Type.BOOL);
            if (node.Children[0].ActualType == ASTNode.Type.ARRACC)
            {
                var accessor = node.Children[0] as ArrAccessNode;
                if (accessor != null && _scope.GetVar(accessor.Symbol).ActualType != ASTNode.Type.ARRAY)
                {
                    throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                                        "non-array object, or using an out-of-range index at line " + node.Line + "\n");
                }
                var actualArray = _scope.GetVar(accessor.Symbol) as ArrayNode;
                ArrayNode orig = actualArray;
                TermNode resolvedInd;
                for (int i = 1; i < accessor.Children.Count - 1; i++)
                {
                    resolvedInd = Resolve(accessor.Children[i], ASTNode.Type.BOOL);
                    if (resolvedInd.ActualType != ASTNode.Type.INT)
                    {
                        throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                            node.Line + "\n");
                    }
                    int ind = (resolvedInd as IntNode).Value;
                    if (ind >= 0 && ind < actualArray.Elements.Count
                        && actualArray[ind].ActualType == ASTNode.Type.ARRAY)
                    {
                        actualArray = actualArray[ind] as ArrayNode;
                    }
                    else
                    {
                        throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                                            "non-array object, or using an out-of-range index at line " + node.Line +
                                            "\n");
                    }
                }
                resolvedInd = Resolve(accessor.Children[accessor.Children.Count - 1], ASTNode.Type.BOOL);
                if (resolvedInd.ActualType != ASTNode.Type.INT)
                {
                    throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                        node.Line + "\n");
                }
                int finalInd = (resolvedInd as IntNode).Value;
                actualArray[finalInd] = resolved;
                _scope.AddVar(accessor.Symbol, orig);
            }
            else
                _scope.AddVar((node.Children[0] as VarNode).Symbol, resolved);
        }


        /// <summary>
        ///     Walks an elsif clause
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Clause result as string</returns>
        private void Walk(ElsifNode node)
        {
            foreach (ASTNode elif in node.Children)
            {
                if (elif != node.Children[0])
                {
                    Walk(elif);
                }
            }
        }


        /// <summary>
        ///     Walks a whole 'if' statement
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Statement result as string</returns>
        private void Walk(IfNode node)
        {
            var cond = Resolve(node.Children[0], ASTNode.Type.BOOL) as BooleanNode;
            if (cond.Value)
            {
                _scope.NewScope();
                foreach (ASTNode n in node.Children)
                {
                    if (!(n is ElsifNode) && !(n is ElseNode) && n != node.Children[0])
                        // we skip the else and elif clauses
                    {
                        Walk(n);
                    }
                }
                _scope.RemoveScope();
            }
            else if (node.Clauses.Count > 0) // if the if statement has elif clauses
            {
                _scope.NewScope();
                foreach (ElsifNode cl in node.Clauses)
                {
                    cond = Resolve(cl.Children[0], ASTNode.Type.BOOL) as BooleanNode;
                    if (cond.Value)
                    {
                        Walk(cl);
                        break; // we only want to walk a single elif clause
                    }
                }
                _scope.RemoveScope();
            }
            else if (node.Else != null)
            {
                Walk(node.Else);
            }
        }

        /// <summary>
        ///     Walks the else part of an 'if' statement
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Clause result as string</returns>
        private void Walk(ElseNode node)
        {
            _scope.NewScope();
            foreach (ASTNode n in node.Children)
            {
                if (n != node.Children[0])
                {
                    Walk(n);
                }
            }
            _scope.RemoveScope();
        }

        /// <summary>
        ///     Walks a 'while' node statement
        /// </summary>
        /// <param name="node">While node</param>
        /// <returns>Whole result of the loop</returns>
        private void Walk(WhileNode node)
        {
            var builder = new StringBuilder();
            var cond = Resolve(node.Children[0], ASTNode.Type.BOOL) as BooleanNode;
            while (cond.Value)
            {
                _scope.NewScope();
                for (int i = 1; i < node.Children.Count; i++) // we skip the condition
                {
                    Walk(node.Children[i]);
                }
                _scope.RemoveScope();
                cond = Resolve(node.Children[0], ASTNode.Type.BOOL) as BooleanNode; // resolve it every time
            }
        }

        /// <summary>
        ///     Provides a method to call functions with parameters provided by the interpreter
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void Walk(FunCallNode node)
        {
            if (node.FunName == "print" && node.Children.Count == 2)
            {
                TermNode resolved = Resolve(node.Children[1], ASTNode.Type.BOOL);
                PrintFun(resolved);
                // should return something meaningful
            }
            if (node.FunName == "append" && node.Children.Count == 3)
            {
                ArrayNode arr = Resolve(node.Children[1], ASTNode.Type.BOOL) as ArrayNode;
                TermNode term = Resolve(node.Children[2], ASTNode.Type.BOOL);
                arr.Elements.Add(term);
            }
            /*
            if (_helper.Contains(node.FunName))
            {

            }
            var b = new StringBuilder();
            for (int i = 1; i < node.Children.Count; i++)
            {
                b.Append(node.Children[i] + " ");
            }
            throw new Exception("RUNTIME ERROR! \n Undefined function call to: " + node.FunName + "\n" +
                                "with args: " + b);
             * */
        }

        private void PrintFun(TermNode node)
        {
            Output.Append(node.ToString() + "\n");
        }

        private void PrintFun(String message)
        {
            Output.Append(message);
        }

        

        /// <summary>
        ///     Resolves an arithmetic node recursively
        /// </summary>
        /// <param name="toRes">Node to be resolved</param>
        /// <param name="currentType">Current strongest type</param>
        /// <returns>Type of the arithmetic expression</returns>
        /// TODO: 
        ///     - eliminate second parameter, because it's useless in the current 
        ///       implementation of resolve
        private TermNode Resolve(ArithmeticNode toRes, ASTNode.Type currentType)
        {
            TermNode left = Resolve(toRes.Children[0], currentType);
            TermNode right = Resolve(toRes.Children[1], currentType);
            return NodeArithmetics.Op(left, right, toRes.op);
        }

        /// <summary>
        ///     Function to resolve FunCall expressions -- experimental
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        private TermNode Resolve(FunCallNode fun, ASTNode.Type currentType)
        {
            var resolvedArgs = new List<TermNode>();
            for (int i = 1; i < fun.Children.Count; i++)
            {
                resolvedArgs.Add(Resolve(fun.Children[i], currentType));
            }
            switch (fun.FunName)
            {
                case "rand":
                    if (fun.Children.Count == 1)
                    {
                        return new IntNode(_rand.Next());
                    }
                    if (fun.Children.Count == 2)
                    {
                        if (resolvedArgs[0].ActualType == ASTNode.Type.INT)
                        {
                            return new IntNode(_rand.Next((resolvedArgs[0] as IntNode).Value));
                        }
                        else
                        {
                            throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line +
                                                "\n");
                        }
                    }
                    else
                    {
                        throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line +
                                            "\n");
                    }
                case "len":
                    var arg = resolvedArgs[0];
                    if (fun.Children.Count == 2 && arg.ActualType == ASTNode.Type.ARRAY)
                    {
                        return new IntNode((arg as ArrayNode).Elements.Count);
                    }
                    else
                    {
                        throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line +
                                            "\n");
                    }
                default:
                    throw new Exception(
                        "RUNTIME ERROR!\n You are either calling a non-existing function, or trying to " +
                        "use a void functions return value.");
            }
        }

        /// <summary>
        ///     Provides an abstraction layer for resolves -- handles double dispatching to the proper functions
        ///     From a grammatical point of view, resolve deals with all expressions, but not statements.
        ///     For statement handlng, see the Walk functions
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
                case ASTNode.Type.INTVAR:
                case ASTNode.Type.BOOLVAR:
                case ASTNode.Type.DOUBLEVAR:
                case ASTNode.Type.STRINGVAR:
                case ASTNode.Type.VAR:
                    return Resolve(toRes as VarNode, currentType);
                case ASTNode.Type.INT:
                case ASTNode.Type.STRING:
                case ASTNode.Type.BOOL:
                case ASTNode.Type.DOUBLE:
                    return Resolve(toRes as TermNode, currentType);
                case ASTNode.Type.ARRAY:
                    return Resolve(toRes as ArrayNode, currentType);
                case ASTNode.Type.ASSIGN:
                    TermNode Resolved = Resolve(toRes.Children[1], ASTNode.Type.BOOL);
                    _scope.AddVar((toRes.Children[0] as VarNode).Symbol, Resolved);
                    return Resolved;
                case ASTNode.Type.ARRACC:
                    return Resolve(toRes as ArrAccessNode, currentType);
                case ASTNode.Type.FUNCALL:
                    return Resolve(toRes as FunCallNode, currentType);
                default:
                    throw new Exception("Unable to resolve ASTNode " + toRes.ActualType.ToString() + " " +
                                        toRes.Parent.ActualType.ToString());
            }
        }

        /// <summary>
        ///     Resolves Array Accessor. The main idea is that apart from the last index, all indices
        ///     need to resolve to arrays, so we iterate through them, and then we resolve the last element
        /// </summary>
        /// <param name="toRes"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        private TermNode Resolve(ArrAccessNode toRes, ASTNode.Type currentType)
        {
            TermNode ret = _scope.GetVar(toRes.Symbol);
            if (ret.ActualType != ASTNode.Type.ARRAY)
            {
                throw new Exception(
                    "RUNTIME ERROR!\n You are trying to use an accessor on a non-array object at line " + toRes.Line +
                    "\n");
            }
            var actualArray = ret as ArrayNode;
            TermNode resolvedInd;
            for (int i = 1; i < toRes.Children.Count - 1; i++)
            {
                resolvedInd = Resolve(toRes.Children[i], currentType);
                if (resolvedInd.ActualType != ASTNode.Type.INT)
                {
                    throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                        toRes.Line + "\n");
                }
                int Ind = (resolvedInd as IntNode).Value;
                if (Ind >= 0 && Ind < actualArray.Elements.Count
                    && actualArray[Ind].ActualType == ASTNode.Type.ARRAY)
                {
                    actualArray = actualArray[Ind] as ArrayNode;
                }
                else
                {
                    throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                                        "non-array object, or using an out-of-range index at line " + toRes.Line + "\n");
                }
            }
            resolvedInd = Resolve(toRes.Children[toRes.Children.Count - 1], currentType);
            if (resolvedInd.ActualType != ASTNode.Type.INT)
            {
                throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                    toRes.Line + "\n");
            }
            return Resolve(actualArray[(resolvedInd as IntNode).Value], currentType);
        }

        /// <summary>
        ///     Resolves array nodes
        /// </summary>
        /// <param name="toRes">An Array node</param>
        /// <param name="currentType"></param>
        /// <returns>Node with elements as terms</returns>
        private TermNode Resolve(ArrayNode toRes, ASTNode.Type currentType)
        {
            foreach (ASTNode node in toRes.Children)
            {
                toRes.Elements.Add(Resolve(node, currentType));
            }
            return toRes;
        }

        /// <summary>
        ///     Functional sugar -- hides the fact that we only need to resolve the right-hand size
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

        /// <summary>
        ///     Deprecated function to resolve variable nodes -- now they're all handled
        ///     as TermNodes in all scopes
        /// </summary>
        /// <param name="toRes"></param>
        /// <param name="currentType"></param>
        /// <returns>Variable's actual value</returns>
        private TermNode Resolve(VarNode toRes, ASTNode.Type currentType)
        {
            TermNode node;
            switch (toRes.ActualType)
            {
                case ASTNode.Type.INTVAR:
                    node = (toRes as IntVarNode).Interpret(_scope);
                    if (ASTNode.Type.INT > currentType)
                    {
                        currentType = ASTNode.Type.INT;
                    }
                    return node;
                case ASTNode.Type.BOOLVAR:
                    node = (toRes as BoolVarNode).Interpret(_scope);
                    if (ASTNode.Type.BOOL > currentType)
                    {
                        currentType = ASTNode.Type.BOOL;
                    }
                    return node;
                case ASTNode.Type.DOUBLEVAR:
                    node = (toRes as DoubleVarNode).Interpret(_scope);
                    if (ASTNode.Type.DOUBLE > currentType)
                    {
                        currentType = ASTNode.Type.DOUBLE;
                    }
                    return node;
                case ASTNode.Type.STRINGVAR:
                    node = (toRes as StringVarNode).Interpret(_scope);
                    if (ASTNode.Type.STRING > currentType)
                    {
                        currentType = ASTNode.Type.STRING;
                    }
                    return node;
                case ASTNode.Type.VAR:
                    return _scope.GetVar(toRes.Symbol);
                case ASTNode.Type.ARRAY:
                    return _scope.GetVar(toRes.Symbol);
                default:
                    return toRes;
            }
        }

/*
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
*/


        /// <summary>
        ///     Calculates the depth of the current AST.
        /// </summary>
        /// <param name="tree">AST root</param>
        /// <param name="curDep">Recursive depth counter</param>
        private void SetTreeDepth(CommonTree tree, int curDep = 0)
        {
            if (curDep > _treeDepth)
            {
                _treeDepth = curDep;
            }
            for (int i = 0; i < tree.ChildCount; i++)
            {
                SetTreeDepth(tree.GetChild(i) as CommonTree, curDep + 1);
            }
        }

        /// <summary>
        ///     Sets up the parser
        /// </summary>
        /// <param name="source">Path to source file</param>
        private void SetParser(string source)
        {
            SStream = new ANTLRStringStream(source);
            Lexer = new aegiscriptLexer(SStream);
            Tokens = new CommonTokenStream(Lexer);
            Parser = new aegiscriptParser(Tokens);

            TokenTypeMediator.SetTokens(Parser.TokenNames);
        }

        public string GetASTTokensAsString(string source)
        {
            SetParser(source);
            return PrettyPrinting(Parser.program().Tree, true, true);
        }


        public string GetASTObjectsAsString(string source)
        {
            SetParser(source);
            return PrettyPrinting(Parser.program().Tree, false, true);
        }

        private string PrettyPrinting(CommonTree tree, bool tokenTextOnly = false, bool printTypes = false)
        {
            var builder = new StringBuilder();
            if (printTypes)
            {
                _treeDepth = 0;
                SetTreeDepth(tree);
            }
            PreOrder(tree, ref builder, -1, tokenTextOnly, printTypes);
            // reason for depth == -1 is that ANTLR builds the tree with the root as null
            return builder.ToString();
        }

        /// <summary>
        ///     Does a pre-order walk of the ast to print out different nodes for debugging reasons
        /// </summary>
        /// <param name="node">Current root</param>
        /// <param name="builder">A StringBuilder for prettyprinting</param>
        /// <param name="depth">Current Depth</param>
        /// <param name="tokenTextOnly">PrintFun only the content contained in the token</param>
        /// <param name="printTypes">PrintFun out the token's type</param>
        private void PreOrder(CommonTree node, ref StringBuilder builder, int depth, bool tokenTextOnly = false,
                              bool printTypes = false)
        {
            AddIndentation(ref builder, depth);
            if (node.Token != null)
            {
                int tokenLengthFactor = node.Token.Text.Length/4;
                int offsetFromMax = 2;
                if (tokenTextOnly)
                {
                    builder.Append(node.Token.Text);
                    tokenLengthFactor = node.Token.Text.Length/4;
                }
                else
                {
                    builder.Append(node.Token);
                    tokenLengthFactor = node.Token.ToString().Length/4;
                    offsetFromMax += 4;
                }
                if (printTypes && node.Type >= 0 && node.Type < Parser.TokenNames.Length)
                {
                    AddIndentation(ref builder, (_treeDepth + offsetFromMax - tokenLengthFactor - depth));
                    builder.Append(Parser.TokenNames[node.Type]);
                    var Node = new ASTNode(node, Parser.TokenNames);
                }
                builder.Append('\n');
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                PreOrder(node.GetChild(i) as CommonTree, ref builder, depth + 1, tokenTextOnly, printTypes);
            }
        }

        /// <summary>
        ///     PrettyPrinting helper, adds depth to the StringBuilder. Not conventional, but for deep trees, this is faster than the standard recursive call.
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

        /// <summary>
        ///     A standard DFS, for visiting and prettyprinting purposes
        /// </summary>
        /// <param name="node">Current root</param>
        /// <param name="depth">Current depth</param>
        /// <param name="v_current">Count of visited nodes</param>
        /// <param name="e_current">Count of "ended" nodes</param>
        private void DepthFirstDescent(CommonTree node, int depth, ref int v_current, ref int e_current)
        {
            if (!_visitHelper.ContainsKey(node))
            {
                _visitHelper.Add(node, new DfsHelper(depth, true));
                _visitHelper[node].Order = v_current++;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                if (!_visitHelper.ContainsKey(node))
                {
                    DepthFirstDescent(node, depth + 1, ref v_current, ref e_current);
                }
            }
            _visitHelper[node].End = e_current++;
        }


        private void DepthFirstDescent(CommonTree node, int depth, ref StringBuilder builder, ref int v_current,
                                       ref int e_current)
        {
            if (!_visitHelper.ContainsKey(node))
            {
                _visitHelper.Add(node, new DfsHelper(depth, true));
                _visitHelper[node].Order = v_current++;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                if (!_visitHelper.ContainsKey(node.GetChild(i) as CommonTree))
                {
                    DepthFirstDescent(node.GetChild(i) as CommonTree, depth + 1, ref builder, ref v_current,
                                      ref e_current);
                }
            }
            _visitHelper[node].End = e_current++;
            if (node.Token != null)
            {
                AddIndentation(ref builder, depth);
                builder.Append(node.Token.Text + "\t\t Depth : " + _visitHelper[node].Depth + "\n");
            }
        }

        public string PrintAST_DFS(string source)
        {
            SetParser(source);
            //SetTreeDepth(parser.program().Tree);
            int depth = 0;
            int end = 0;
            var builder = new StringBuilder();
            DepthFirstDescent(Parser.program().Tree, 0, ref builder, ref depth, ref end);
            return builder.ToString();
        }

        private class DfsHelper
        {
            public DfsHelper(int depth, bool visited)
            {
                Depth = depth;
                Visited = visited;
            }

            public int Depth { get; private set; }
            public int Order { get; set; }
            public int End { get; set; }
            private bool Visited { get; set; }
        }
    }
}