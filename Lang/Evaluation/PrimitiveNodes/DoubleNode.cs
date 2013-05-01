using System;
using AEGIScript.Lang.Scoping;
using Antlr.Runtime.Tree;
using System.Globalization;

namespace AEGIScript.Lang.Evaluation.PrimitiveNodes
{
    class DoubleNode : TermNode
    {
        public DoubleNode(CommonTree tree, String content) : base(tree)
        {
            Value = Double.Parse(content, CultureInfo.InvariantCulture);
            ActualType = Type.Double;
        }

        public DoubleNode(Double value)
        {
            Value = value;
            ActualType = Type.Double;
        }

        public Double Interpret(SymbolTable symbols)
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
