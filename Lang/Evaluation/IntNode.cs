using System.Globalization;
using Antlr.Runtime.Tree;
using System;

namespace AEGIScript.Lang.Evaluation
{
    class IntNode : TermNode 
    {
        public IntNode(CommonTree tree, String content) : base(tree, content)
        {
            // check for overflow
            Value = Int32.Parse(content);
            ActualType = Type.Int;
        }

        public IntNode(Int32 value)
        {
            Value = value;
            ActualType = Type.Int;
        }

        public int Value { get; private set; }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    class UInt32Node : TermNode
    {
        public UInt32Node(UInt32 value) 
            : base()
        {
            ActualType = Type.UInt32;
            Value = value;
        }

        public UInt32 Value { get; private set; }
    }

    class UInt16Node : TermNode
    {
        public UInt16Node(UInt16 value)
            : base()
        {
            ActualType = Type.UInt16;
            Value = value;
        }

        public UInt32 Value { get; private set; }
    }

    class NegativeNode : TermNode
    {
        public NegativeNode(CommonTree tree, String content)
            : base(tree, content)
        {
            ActualType = Type.Negative;
        }
    }
}
