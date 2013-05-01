using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation.StatementNodes
{
    class WhileNode : ASTNode
    {
        public WhileNode(CommonTree tree) : base(tree)
        {
            ActualType = Type.While;
        }
    }
}
