using System;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation
{
    class FieldAccessNode : TermNode
    {
        public FieldAccessNode(CommonTree tree, String content) :base(tree)
        {
            
        }

        public TermNode Value { get; set; }
    }
}
