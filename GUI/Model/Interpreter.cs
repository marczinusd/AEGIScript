using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using AEGIScript.IO;
using AEGIScript.Lang.ANTLR;
using AEGIScript.Lang.Evaluation;
using AEGIScript.Lang.Evaluation.AEGISNodes;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.Helpers;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using AEGIScript.Lang.Evaluation.StatementNodes;
using AEGIScript.Lang.Exceptions;
using AEGIScript.Lang.Scoping;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using ELTE.AEGIS.Core;
using ELTE.AEGIS.Core.Geometry;
using ELTE.AEGIS.IO;

namespace AEGIScript.GUI.Model
{
    internal class Interpreter
    {
        #region Private Fields
        private readonly HashSet<String> _constructors;
        private readonly Random _rand = new Random();
        private readonly Scope _scope = new Scope();
        private readonly Dictionary<CommonTree, DfsHelper> _visitHelper = new Dictionary<CommonTree, DfsHelper>();
        private AsyncOperation _async;
        private DateTime _beginTime;
        private bool _errorsIgnored;
        private Boolean _hasAsync;
        private int _treeDepth;

        /// <summary>
        ///     Fields provided by ANTLR
        /// </summary>
        private ANTLRStringStream SStream { get; set; }
        private aegiscriptLexer Lexer { get; set; }
        private CommonTokenStream Tokens { get; set; }
        private aegiscriptParser Parser { get; set; }

        private CancellationToken Token { get; set; }
        private int CurrentNode { get; set; }
        private int AllNodes { get; set; }

        #endregion

        public Interpreter()
        {
            Output = new StringBuilder();
            _constructors = new HashSet<string> {"Array", "ShapeFileReader", "TiffReader", "GeoTiffReader", "GetGeometryFactory"};
        }

        #region Public Fields

        public StringBuilder Output { get; private set; }
        public event ProgressChangedEventHandler ProgressChanged;
        #endregion 

        #region Interpreter functions

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
        ///     Interface provided for the ViewModel for UI actions
        /// </summary>
        /// <param name="source">Source code to interpret</param>
        /// <param name="fromImmediate">True, if we need to interpret immediate code</param>
        /// <returns>Interpreter output</returns>
        public void Walk(string source, bool fromImmediate = false)
        {
            Output.Clear();
            Token = new CancellationToken();
            _errorsIgnored = fromImmediate;
            if (!fromImmediate)
            {
                _scope.Clear();
            }
            SetParser(source);

            var ret = Parser.program();

            // can't really determine where the problem came from
            // ANTLR suppresses errors during parsing and lexing
            var text = ret.Tree.Text;
            if (CheckANTLRErrors() > 0)
            {
                Output.Append("Syntax error!");
                return;
            }
            Walk(ret.Tree);
        }

        public void WalkAsync(String source, CancellationToken token, AsyncOperation async,
                                 bool errorsIgnored = false)
        {
            _hasAsync = true;
            _async = async;
            Token = token;
            _errorsIgnored = errorsIgnored;
            Output.Clear();
            _scope.Clear();
            SetParser(source);
            Token.ThrowIfCancellationRequested();
            var ret = Parser.program();
            if (CheckANTLRErrors() > 0)
            {
                Output.Append("Syntax error!");
                return;
            }
            Walk(ret.Tree);
            _hasAsync = false;
        }

        //      ANTLR suppresses errors during parsing and lexing
        /// <summary>
        ///     Trivial checks to ANTLR's output --
        ///     can't really determine where the problem came from
        /// </summary>
        private int CheckANTLRErrors()
        {
            return Lexer.NumberOfSyntaxErrors + Parser.NumberOfSyntaxErrors;
            //if (Lexer.NumberOfSyntaxErrors > 0)
            //{
            //    //Print(this, new PrintEventArgs("Lexical error!"));
            //    Output.Append("Lexical error!");
            //}
            //if (Parser.NumberOfSyntaxErrors > 0)
            //{
            //    //Print(this, new PrintEventArgs("Syntax error!"));
            //    Output.Append("Syntax error!");
            //}
        }

        private void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (e != null)
            {
                ProgressChanged(this, e);
            }
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
                output = PrettyPrinting(Parser.program().Tree);
            } // check for parsing errors
            catch (InvalidOperationException ex)
            {
                output = "RUNTIME ERROR!\n Error in an external DLL with message: \n " + ex.Message + ", TargetSite: " +
                         ex.TargetSite + ", Source: " + ex.Source;
            }

            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }
#endregion

        #region Walks

        /// <summary>
        ///     Provides a layer of abstraction for walking ASTNodes -- handles double dispatching gracefully
        ///     From a grammatical point of view, walk handles actual statements
        /// </summary>
        /// <param name="node"></param>
        /// <returns>String representation for debug purposes</returns>
        private void Walk(ASTNode node)
        {
            switch (node.ActualType)
            {
                case ASTNode.Type.Assign:
                    Walk(node as AssignNode);
                    break;
                case ASTNode.Type.While:
                    Walk(node as WhileNode);
                    break;
                case ASTNode.Type.If:
                    Walk(node as IfNode);
                    break;
                case ASTNode.Type.Elif:
                    Walk(node as ElsifNode);
                    break;
                case ASTNode.Type.Else:
                    Walk(node as ElseNode);
                    break;
                case ASTNode.Type.FunCall:
                    Walk(node as FunCallNode);
                    break;
                default:
                    throw new Exception("RUNTIME ERROR! \n Invalid statement on line: " + node.Line);
            }
        }

        /// <summary>
        ///     The soul of the interpreter -- walks the AST and interprets it
        /// </summary>
        /// <param name="node">Root of the AST</param>
        private void Walk(BeginNode node)
        {
            _beginTime = DateTime.UtcNow;
            AllNodes = node.Children.Count;
            for (int i = 0; i < node.Children.Count; i++)
            {
                CurrentNode = i;
                ASTNode n = node.Children[i];
                try
                {
                    Token.ThrowIfCancellationRequested();
                    Walk(n);
                    ReportProgress();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (InvalidOperationException ex)
                {
                    PrintLineFun("RUNTIME ERROR!\n Error in an external DLL with message: \n " + ex.Message +
                                 ", TargetSite: " + ex.TargetSite + ", Source: " + ex.Source);

                    if (!_errorsIgnored)
                        return;
                }
                catch (Exception ex)
                {
                    PrintLineFun(ex.Message);

                    if (!_errorsIgnored)
                        return;
                }
            }
            ReportCompletionTime();
        }


        private void ReportCompletionTime()
        {
            TimeSpan elapsedTime = DateTime.UtcNow - _beginTime;
            double asDouble = Math.Truncate(elapsedTime.TotalSeconds*10000)/10000;
            Output.Insert(0, "Successfully finished in: " + asDouble.ToString(CultureInfo.InvariantCulture) +
                             " second(s). \n\n");
        }


        /// <summary>
        ///     Runs an async progress report
        /// </summary>
        private void ReportProgress()
        {
            if (_hasAsync)
            {
                int progress = (100*(CurrentNode + 1)/AllNodes);
                _async.Post
                    (o =>
                        {
                            var e = o as ProgressChangedEventArgs;
                            OnProgressChanged(e);
                        },
                     new InterpreterProgressChangedArgs(progress, _async.UserSuppliedState, Output.ToString())
                    );
            }
        }

        /// <summary>
        ///     Internal use only
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
            catch (OperationCanceledException)
            {
                Output.Clear();
                _scope.Clear();
                Output.Append("OPERATION CANCELLED BY USER.");
            }
            //catch (InvalidOperationException ex)
            //{
            //    PrintLineFun("RUNTIME ERROR!\n Error in an external DLL with message: \n " + ex.Message +
            //                 ", TargetSite: " + ex.TargetSite + ", Source: " + ex.Source);
            //}
            //catch (Exception ex)
            //{
            //    PrintLineFun(ex.Message);
            //}
        }

        /// <summary>
        ///     Performs basic assignment
        /// </summary>
        /// <param name="node">An AssignNode</param>
        /// <returns>Assigned variable as string</returns>
        private void Walk(AssignNode node)
        {
            TermNode resolved = Resolve(node.Children[1]);
            if (node.Children[0].ActualType == ASTNode.Type.ArrAcc)
            {
                var accessor = node.Children[0] as ArrAccessNode;
                if (accessor != null && _scope.GetVar(accessor.Symbol).ActualType != ASTNode.Type.Array)
                {
                    throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                                        "non-array object, or using an out-of-range index at line " + node.Line + "\n");
                }
                var actualArray = _scope.GetVar(accessor.Symbol) as ArrayNode;
                ArrayNode orig = actualArray;
                TermNode resolvedInd;
                for (int i = 1; i < accessor.Children.Count - 1; i++)
                {
                    resolvedInd = Resolve(accessor.Children[i]);
                    if (resolvedInd.ActualType != ASTNode.Type.Int)
                    {
                        throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                            node.Line + "\n");
                    }
                    int ind = ((IntNode) resolvedInd).Value;
                    if (ind >= 0 && ind < actualArray.Elements.Count
                        && actualArray[ind].ActualType == ASTNode.Type.Array)
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
                resolvedInd = Resolve(accessor.Children[accessor.Children.Count - 1]);
                if (resolvedInd.ActualType != ASTNode.Type.Int)
                {
                    throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                        node.Line + "\n");
                }
                int finalInd = ((IntNode) resolvedInd).Value;
                actualArray[finalInd] = resolved;
                _scope.AddVar(accessor.Symbol, orig);
            }
            else
                _scope.AddVar(((VarNode) node.Children[0]).Symbol, resolved);
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
            var cond = Resolve(node.Children[0]) as BooleanNode;
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
                    cond = Resolve(cl.Children[0]) as BooleanNode;
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
                Walk(n);
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
            var cond = Resolve(node.Children[0]) as BooleanNode;
            while (cond.Value)
            {
                _scope.NewScope();
                Token.ThrowIfCancellationRequested();
                for (int i = 1; i < node.Children.Count; i++) // we skip the condition
                {
                    Walk(node.Children[i]);
                }
                _scope.RemoveScope();
                cond = Resolve(node.Children[0]) as BooleanNode; // resolve it every time
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
                TermNode resolved = Resolve(node.Children[1]);
                PrintFun(resolved);
                // should return something meaningful
            }
            else if (node.FunName == "append" && node.Children.Count == 3)
            {
                var arr = Resolve(node.Children[1]) as ArrayNode;
                TermNode term = Resolve(node.Children[2]);
                arr.Elements.Add(term);
            }
            else if (node.FunName == "printline" && node.Children.Count == 2)
            {
                TermNode resolved = Resolve(node.Children[1]);
                PrintLineFun(resolved);
            }
            else if (node.FunName == "close" && node.Children.Count == 2)
            {
                TermNode resolved = Resolve(node.Children[1]);
                if (resolved.ActualType == ASTNode.Type.File)
                {
                    ((FileNode)resolved).Close();
                }
                else throw ExceptionGenerator.BadArguments(node);
            }
            else
            {
                throw new Exception("RUNTIME ERROR! \n Undefined function call at line " + node.Line + " \n");
            }
        }

        #endregion

        #region Resolves

        /// <summary>
        ///     Provides an abstraction layer for resolves -- handles double dispatching to the proper functions
        ///     From a grammatical point of view, resolve deals with all expressions, but not statements.
        ///     For statement handlng, see the Walk functions
        /// </summary>
        /// <param name="toRes"></param>
        /// <returns></returns>
        private TermNode Resolve(ASTNode toRes)
        {
            switch (toRes.ActualType)
            {
                case ASTNode.Type.Arith:
                    return Resolve(toRes as ArithmeticNode);
                case ASTNode.Type.Var:
                    return Resolve(toRes as VarNode);
                case ASTNode.Type.Int:
                case ASTNode.Type.String:
                case ASTNode.Type.Bool:
                case ASTNode.Type.Double:
                    return Resolve(toRes as TermNode);
                case ASTNode.Type.Array:
                    return Resolve(toRes as ArrayNode);
                case ASTNode.Type.Assign:
                    TermNode resolved = Resolve(toRes.Children[1]);
                    _scope.AddVar(((VarNode) toRes.Children[0]).Symbol, resolved);
                    return resolved;
                case ASTNode.Type.ArrAcc:
                    return Resolve(toRes as ArrAccessNode);
                case ASTNode.Type.FunCall:
                    return Resolve(toRes as FunCallNode);
                case ASTNode.Type.FieldAccess:
                    return Resolve(toRes as FieldAccessNode);
                case ASTNode.Type.Negation:
                    return Resolve(toRes as NegationNode);
                case ASTNode.Type.Negative:
                    return Resolve(toRes as NegativeNode);
                default:
                    throw new Exception("Unable to resolve ASTNode " + toRes.ActualType.ToString() + " " +
                                        toRes.Parent.ActualType.ToString());
            }
        }

        /// <summary>
        ///     Resolves an arithmetic node recursively
        /// </summary>
        /// <param name="toRes">Node to be resolved</param>
        /// <returns>Type of the arithmetic expression</returns>
        /// TODO: 
        /// - eliminate second parameter, because it's useless in the current 
        /// implementation of resolve
        private TermNode Resolve(ArithmeticNode toRes)
        {
            TermNode left = Resolve(toRes.Children[0]);
            TermNode right = Resolve(toRes.Children[1]);
            return NodeArithmetics.Op(left, right, toRes.Op);
        }

        /// <summary>
        ///     Function to resolve FunCall expressions
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        private TermNode Resolve(FunCallNode fun)
        {
            var resolvedArgs = new List<TermNode>();
            for (int i = 1; i < fun.Children.Count; i++)
            {
                resolvedArgs.Add(Resolve(fun.Children[i]));
            }
            if (_constructors.Contains(fun.FunName))
            {
                fun.ResolvedArgs = resolvedArgs;
                return Constructor.Construct(fun);
            }
            return RunBuiltinFuns(fun, resolvedArgs);
        }


        /// <summary>
        ///     Acts as the standard library of the language. Should extract to seperate module, 
        ///     see: Lang/Evaluation/Helpers/Library
        /// </summary>
        /// <param name="fun">Function to call</param>
        /// <param name="resolvedArgs">Actual arguments</param>
        /// <returns>Result of the call</returns>
        private TermNode RunBuiltinFuns(FunCallNode fun, List<TermNode> resolvedArgs)
        {
            switch (fun.FunName)
            {
                case "rand":
                    if (fun.Children.Count == 1)
                    {
                        return new IntNode(_rand.Next());
                    }
                    if (fun.Children.Count == 2)
                    {
                        if (resolvedArgs[0].ActualType == ASTNode.Type.Int)
                        {
                            return new IntNode(_rand.Next((resolvedArgs[0] as IntNode).Value));
                        }
                        throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line +
                                            "\n");
                    }
                    throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line +
                                        "\n");
                case "len":
                    TermNode arg = resolvedArgs[0];
                    if (fun.Children.Count == 2 && arg.ActualType == ASTNode.Type.Array)
                    {
                        return new IntNode((arg as ArrayNode).Elements.Count);
                    }
                    throw new Exception("RUNTIME ERROR!\n Function called with invalid args at line" + fun.Line +
                                        "\n");
                case "ReadWKT":
                    IReferenceSystem referenceSystem = GeometryFactory.ReferenceSystem;
                    if (resolvedArgs[0].ActualType == ASTNode.Type.String &&
                        File.Exists((resolvedArgs[0] as StringNode).Value) && resolvedArgs.Count == 1)
                    {
                        try
                        {
                            String read = SourceIO.ReadWKT((resolvedArgs[0] as StringNode).Value);
                            IGeometry res = GeometryConverter.ToGeometry(read, referenceSystem);
                            return new GeometryNode(res);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("RUNTIME ERROR!\nWell-known text read failed with message: " +
                                                ex.Message + "\n at line: " + fun.Line);
                        }
                    }
                    throw ExceptionGenerator.BadArguments(fun, new[] { ASTNode.Type.String });
                case "Dir":
                    if (resolvedArgs.Count == 0)
                    {
                        List<TermNode> files = Directory
                            .GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                            .Select( s => new StringNode(s))
                            .Cast<TermNode>()
                            .ToList();
                        return new ArrayNode(files);
                    }
                    if (resolvedArgs.Count == 1 && resolvedArgs[0].ActualType == ASTNode.Type.String)
                    {
                        List<TermNode> files = Directory
                            .GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                      ((StringNode) resolvedArgs[0]).Value)
                            .Select(s => new StringNode(s))
                            .Cast<TermNode>()
                            .ToList();
                        return new ArrayNode(files);
                    }
                    if (resolvedArgs.Count == 2 && resolvedArgs[0].ActualType == ASTNode.Type.String &&
                        resolvedArgs[1].ActualType == ASTNode.Type.String)
                    {
                        String path = ((StringNode) resolvedArgs[0]).Value;
                        String filter = ((StringNode) resolvedArgs[1]).Value;
                        var Files = Directory.GetFiles(path, filter);
                        List<TermNode> files = Files.Select(s => new StringNode(s)).Cast<TermNode>().ToList();
                        return new ArrayNode(files);
                    }
                    else throw ExceptionGenerator.BadArguments(fun);
                case "Open":
                    if (resolvedArgs.Count == 1 && resolvedArgs[0].ActualType == ASTNode.Type.String)
                    {
                        string path = ((StringNode) resolvedArgs[0]).Value;
                        if (!File.Exists(path))
                        {
                            throw ExceptionGenerator.EndOfStream();
                        }
                        var stream = new FileStream(path, FileMode.Open);
                        return new FileNode(stream);
                    }
                    throw ExceptionGenerator.BadArguments(fun);
                default:
                    throw new Exception(
                        "RUNTIME ERROR!\n You are either calling a non-existing function, or trying to " +
                        "use a void functions return value.");
            }
        }

        private TermNode Resolve(FieldAccessNode node)
        {
            if (node.Children[0].ActualType != ASTNode.Type.Var)
            {
                return Resolve(node.Children[0]);
            }

            var first = node.Children[0] as VarNode;
            TermNode resolved = Resolve(first);
            for (int i = 1; i < node.Children.Count; i++)
            {
                resolved = ResolveGenFun(resolved, node.Children[i] as FunCallNode);
            }
            return resolved;
        }

        private TermNode ResolveGenFun(TermNode caller, FunCallNode node)
        {
            var args = new List<TermNode>();
            for (int i = 1; i < node.Children.Count; i++)
            {
                args.Add(Resolve(node.Children[i]));
            }
            node.ResolvedArgs = args;
            return caller.CallFun(node);
        }

        private TermNode Resolve(NegationNode node)
        {
            return NodeArithmetics.Op(node, Resolve(node.Children[0]), ArithmeticNode.Operator.ADD);
        }

        private TermNode Resolve(NegativeNode node)
        {
            return NodeArithmetics.Op(node, Resolve(node.Children[0]), ArithmeticNode.Operator.ADD);
        }

        /// <summary>
        ///     Resolves Array Accessor. The main idea is that apart from the last index, all indices
        ///     need to resolve to arrays, so we iterate through them, and then we resolve the last element
        /// </summary>
        /// <param name="toRes"></param>
        /// <returns></returns>
        private TermNode Resolve(ArrAccessNode toRes)
        {
            TermNode ret = _scope.GetVar(toRes.Symbol);
            if (ret.ActualType != ASTNode.Type.Array)
            {
                throw new Exception(
                    "RUNTIME ERROR!\n You are trying to use an accessor on a non-array object at line " + toRes.Line +
                    "\n");
            }
            var actualArray = ret as ArrayNode;
            TermNode resolvedInd;
            for (int i = 1; i < toRes.Children.Count - 1; i++)
            {
                resolvedInd = Resolve(toRes.Children[i]);
                if (resolvedInd.ActualType != ASTNode.Type.Int)
                {
                    throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                        toRes.Line + "\n");
                }
                int ind = (resolvedInd as IntNode).Value;
                if (ind >= 0 && ind < actualArray.Elements.Count
                    && actualArray[ind].ActualType == ASTNode.Type.Array)
                {
                    actualArray = actualArray[ind] as ArrayNode;
                }
                else
                {
                    throw new Exception("RUNTIME ERROR!\n You are either trying to use an accessor on a " +
                                        "non-array object, or using an out-of-range index at line " + toRes.Line + "\n");
                }
            }
            resolvedInd = Resolve(toRes.Children[toRes.Children.Count - 1]);
            if (resolvedInd.ActualType != ASTNode.Type.Int)
            {
                throw new Exception("RUNTIME ERROR!\n You are trying to use a non-integer accessor on line " +
                                    toRes.Line + "\n");
            }
            try
            {
                var toReturn = Resolve(actualArray[(resolvedInd as IntNode).Value]);
                return toReturn;
            }
            catch (IndexOutOfRangeException)
            {
                throw ExceptionGenerator.IndexOutOfRange(toRes);
            }

        }

        /// <summary>
        ///     Resolves array nodes
        /// </summary>
        /// <param name="toRes">An Array node</param>
        /// <returns>Node with elements as terms</returns>
        private TermNode Resolve(ArrayNode toRes)
        {
            foreach (ASTNode node in toRes.Children)
            {
                toRes.Elements.Add(Resolve(node));
            }
            return toRes;
        }

        private TermNode Resolve(TermNode toRes)
        {
            return toRes;
        }

        /// <summary>
        ///     Deprecated function to resolve variable nodes -- now they're all handled
        ///     as TermNodes in all scopes
        /// </summary>
        /// <param name="toRes"></param>
        /// <returns>Variable's actual value</returns>
        private TermNode Resolve(VarNode toRes)
        {
            switch (toRes.ActualType)
            {
                case ASTNode.Type.Var:
                case ASTNode.Type.Array:
                    return _scope.GetVar(toRes.Symbol);
                default:
                    return toRes;
            }
        }

        #endregion

        #region Printing functions

        private void PrintFun(TermNode node)
        {
            Output.Append(node);
            ReportProgress();
        }

        private void PrintLineFun(TermNode node)
        {
            Output.Append("\n" + node);
            ReportProgress();
        }

        private void PrintLineFun(String message)
        {
            Output.Append("\n" + message);
            ReportProgress();
        }

        #endregion

        #region Pretty printing functions to debug AST

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
                int offsetFromMax = 2;
                int tokenLengthFactor;
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
        /// <param name="vCurrent">Count of visited nodes</param>
        /// <param name="eCurrent">Count of "ended" nodes</param>
        private void DepthFirstDescent(CommonTree node, int depth, ref int vCurrent, ref int eCurrent)
        {
            if (!_visitHelper.ContainsKey(node))
            {
                _visitHelper.Add(node, new DfsHelper(depth, true));
                _visitHelper[node].Order = vCurrent++;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                if (!_visitHelper.ContainsKey(node))
                {
                    DepthFirstDescent(node, depth + 1, ref vCurrent, ref eCurrent);
                }
            }
            _visitHelper[node].End = eCurrent++;
        }


        private void DepthFirstDescent(CommonTree node, int depth, ref StringBuilder builder, ref int vCurrent,
                                       ref int eCurrent)
        {
            if (!_visitHelper.ContainsKey(node))
            {
                _visitHelper.Add(node, new DfsHelper(depth, true));
                _visitHelper[node].Order = vCurrent++;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                if (!_visitHelper.ContainsKey(node.GetChild(i) as CommonTree))
                {
                    DepthFirstDescent(node.GetChild(i) as CommonTree, depth + 1, ref builder, ref vCurrent,
                                      ref eCurrent);
                }
            }
            _visitHelper[node].End = eCurrent++;
            if (node.Token != null)
            {
                AddIndentation(ref builder, depth);
                builder.Append(node.Token.Text + "\t\t Depth : " + _visitHelper[node].Depth + "\n");
            }
        }

        public string PrintAST_DFS(string source)
        {
            SetParser(source);
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

            private bool Visited { get;  set; }
        }

        #endregion
    }
}