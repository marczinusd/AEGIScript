using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;


namespace AEGIScript.Lang.Evaluation
{
    class StringNode : TermNode
    {
        public StringNode(CommonTree tree, String content) : base(tree, content)
        {
            Value = content.Trim(new char[]{'"'});
            ActualType = Type.String;
        }

        public StringNode(String value)
        {
            Value = value;
            ActualType = Type.String;
        }

        public String Interpret(SymbolTables.SymbolTable symbols)
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
            //return '"' + Value + '"';
            return Value;
        }
    }
}
