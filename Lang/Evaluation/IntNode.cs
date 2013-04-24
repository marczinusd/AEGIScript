using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation
{
    class IntNode : TermNode 
    {
        public IntNode(CommonTree tree, String content) : base(tree, content)
        {
            // check for overflow
            Value = Int32.Parse(content);
            ActualType = Type.INT;
        }

        public IntNode(Int32 value)
        {
            Value = value;
            ActualType = Type.INT;
        }

        public int Value { get; private set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
