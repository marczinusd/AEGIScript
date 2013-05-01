using System;
<<<<<<< HEAD:Lang/Evaluation/PrimitiveNodes/StringNode.cs
<<<<<<< HEAD:Lang/Evaluation/PrimitiveNodes/StringNode.cs
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Scoping;
=======
>>>>>>> 02d2e234ae3a1038fef2923d05ff58208dfe66a6:Lang/Evaluation/StringNode.cs
=======
<<<<<<< HEAD:Lang/Evaluation/StringNode.cs
=======
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Scoping;
>>>>>>> Project restructured, geofactory support added:Lang/Evaluation/PrimitiveNodes/StringNode.cs
>>>>>>> detach:Lang/Evaluation/PrimitiveNodes/StringNode.cs
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
