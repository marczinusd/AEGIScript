using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Evaluation
{
    class BooleanNode : TermNode
    {
        public BooleanNode(CommonTree tree, string content)
            : base(tree, content)
        {
            Value = Boolean.Parse(content);
            ActualType = Type.BOOL;
        }

        public BooleanNode(Boolean value)
        {
            Value = value;
            ActualType = Type.BOOL;
        }


        public Boolean Interpret(SymbolTables.SymbolTable symbols)
        {
            return true;
        }

        public Boolean Value { get; private set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
