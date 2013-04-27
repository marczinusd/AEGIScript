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
}
