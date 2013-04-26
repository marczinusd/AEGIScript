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
            ActualType = Type.STRING;
        }

        public StringNode(String value)
        {
            Value = value;
            ActualType = Type.STRING;
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
                    if (func.ResolvedArgs.Count == 0)
                    {
                        return Count();
                    }
                    else throw new Exception(func.BadCallMessage());
                default:
                    throw new Exception(func.BadCallMessage());
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
