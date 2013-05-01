using System;
using System.Linq;
using AEGIScript.Lang.Evaluation.AEGISNodes;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Exceptions;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation.PrimitiveNodes
{
    internal class TermNode : ASTNode
    {
        public TermNode(CommonTree tree) : base(tree) { }

        /// <summary>
        ///     Constructor for primitive types
        /// </summary>
        protected TermNode() { }


        /// <summary>
        ///     Provides an interface for the interpreter to call functions defined by the nodes
        /// </summary>
        /// <param name="func">Function node</param>
        /// <returns>Result of the function call</returns>
        public virtual TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Type":
                    return Call(func, () => new TypeNode(ActualType));

                default:
                    throw ExceptionGenerator.UndefinedFunction(func, ActualType);
            }
        }



        protected TermNode Call(FunCallNode func, Func<TermNode> funToCall)
        {
            if (func.ResolvedArgs.Count == 0)
            {
                return funToCall.Invoke();
            }
            throw ExceptionGenerator.UndefinedFunction(func, ActualType);
        }

        /// <summary>
        ///     Provides an easy to use interface for calling functions in TermNode's CallFun method.
        /// </summary>
        /// <typeparam name="T">Parameter type of the function</typeparam>
        /// <param name="funSig">Signature of the function to call</param>
        /// <param name="actualSig">Actual parameter types of the calling node</param>
        /// <param name="fnode">Calling node</param>
        /// <param name="func">Function to call</param>
        /// <returns>Resulting TermNode.</returns>
        protected TermNode Call<T>(Type[] funSig, Type[] actualSig, FunCallNode fnode, Func<T, TermNode> func)
            where T : class
        {
            if (funSig.Length != actualSig.Length)
                throw ExceptionGenerator.BadArity(fnode);

            if (!MatchesSignature(funSig, actualSig))
                throw ExceptionGenerator.BadArguments(fnode, funSig);

            var arg = fnode.ResolvedArgs[0] as T;
            return func.Invoke(arg);
        }

        /// <summary>
        ///     Provides an easy to use interface for calling functions in TermNode's CallFun method.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <param name="funSig">Signature of the function to call</param>
        /// <param name="actualSig">Actual parameter types of the calling node</param>
        /// <param name="fnode">Calling node</param>
        /// <param name="func">Function to call</param>
        /// <returns>Resulting TermNode.</returns>
        protected TermNode Call<T1, T2>(Type[] funSig, Type[] actualSig, FunCallNode fnode,
                                        Func<T1, T2, TermNode> func)
            where T1 : class
            where T2 : class
        {
            if (funSig.Length != actualSig.Length)
                throw ExceptionGenerator.BadArity(fnode);

            if (!MatchesSignature(funSig, actualSig))
                throw ExceptionGenerator.BadArguments(fnode, funSig);

            var arg1 = fnode.ResolvedArgs[0] as T1;
            var arg2 = fnode.ResolvedArgs[1] as T2;
            return func.Invoke(arg1, arg2);
        }

        /// <summary>
        ///     Provides an easy to use interface for calling functions in TermNode's CallFun method.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <typeparam name="T3">Type of the third parameter</typeparam>
        /// <param name="funSig">Signature of the function to call</param>
        /// <param name="actualSig">Actual parameter types of the calling node</param>
        /// <param name="fnode">Calling node</param>
        /// <param name="func">Function to call</param>
        /// <returns>Resulting TermNode.</returns>
        protected TermNode Call<T1, T2, T3>(Type[] funSig, Type[] actualSig, FunCallNode fnode,
                                            Func<T1, T2, T3, TermNode> func)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (funSig.Length != actualSig.Length)
                throw ExceptionGenerator.BadArity(fnode);

            if (!MatchesSignature(funSig, actualSig))
                throw ExceptionGenerator.BadArguments(fnode, funSig);

            var arg1 = fnode.ResolvedArgs[0] as T1;
            var arg2 = fnode.ResolvedArgs[0] as T2;
            var arg3 = fnode.ResolvedArgs[0] as T3;
            return func.Invoke(arg1, arg2, arg3);
        }

        protected bool MatchesSignature(Type[] funSig, Type[] argSig)
        {
            return !argSig.Where((t, i) => funSig[i] != t && funSig[i] != Type.Any).Any();
        }
    }
}