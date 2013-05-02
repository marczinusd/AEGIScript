using System;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Scoping;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation.PrimitiveNodes
{
    class StringNode : TermNode
    {
        public StringNode(CommonTree tree, String content) : base(tree)
        {
            Value = content.Trim(new[]{'"'});
            ActualType = Type.String;
        }

        public StringNode(String value)
        {
            Value = value;
            ActualType = Type.String;
        }

        public String Interpret(SymbolTable symbols)
        {
            return "";
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Count":
                    return Call(func, Count);
                default:
                    return base.CallFun(func);
            }
        }


        private IntNode Count()
        {
            return new IntNode(Value.Length);
        }

        public string Value { get; private set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
