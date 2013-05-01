using System;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation.ExpressionNodes
{
    class FieldAccessNode : TermNode
    {
        public FieldAccessNode(CommonTree tree, String content) :base(tree)
        {
            
        }

        public TermNode Value { get; set; }
    }
}
