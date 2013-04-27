using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Tree;


namespace AEGIScript.Lang.Evaluation
{
    /// <summary>
    ///     Store as termnode?
    /// </summary>
    internal class FunCallNode : ASTNode
    {
        public FunCallNode(CommonTree tree) : base(tree)
        {
            ActualType = Type.FunCall;
            FunName = ((CommonTree) tree.Children[0]).Text;
        }


        public TermNode ReturnValue { get; set; }
        public string FunName { get; private set; }
        public FunCallAttributes Attributes { get; set; }
        public List<TermNode> ResolvedArgs { get; set; } 

        public Boolean MatchesSignature(List<Type> types)
        {
            if (types == null || Attributes.Signature == null || types.Count != Attributes.Signature.Count)
            {
                return false;
            }
            return !types.Where((t, i) => t != Attributes.Signature[i]).Any();
        }

        public virtual void Call(List<TermNode> args, Type caller)
        {
            ResolvedArgs = args;
        }

        public String BadCallMessage()
        {
            return "RUNTIME ERROR! Invalid function call with name: " + FunName + " at line: " + Line;
        }

        public class FunCallAttributes
        {
            private List<Type> _signature;

            public FunCallAttributes()
            {
            }

            public FunCallAttributes(Type retType, List<Type> argList)
            {
                Signature = argList;
                ReturnType = retType;
                IsVoid = false;
                NoParam = false;
            }

            public FunCallAttributes(Type retType)
            {
                ReturnType = retType;
                IsVoid = false;
                NoParam = true;
            }

            public FunCallAttributes(List<Type> argList)
            {
                Signature = argList;
                IsVoid = true;
                NoParam = false;
            }

            public Type ReturnType { get; set; }
            public List<Type> Signature
            {
                get { return _signature; }
                set 
                { 
                    _signature = value;
                    NoParam = _signature.Count > 0;
                }
            }

            public Boolean IsVoid { get; private set; }
            public Boolean NoParam { get; private set; }
        }
    }
}