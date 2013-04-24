using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;
using System.Globalization;

namespace AEGIScript.Lang.Evaluation
{
    class DoubleNode : TermNode
    {
        public DoubleNode(CommonTree tree, String content) : base(tree, content)
        {
            Value = Double.Parse(content, CultureInfo.InvariantCulture);
            ActualType = Type.DOUBLE;
        }

        public DoubleNode(Double value)
        {
            Value = value;
            ActualType = Type.DOUBLE;
        }

        public Double Interpret(SymbolTables.SymbolTable symbols)
        {
            return Value;
        }

        public double Value { get; private set; }

        public override string ToString()
        {
            return Value.ToString("F09");
        }
    }
}
