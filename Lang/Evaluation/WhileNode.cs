using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation
{
    class WhileNode : ASTNode
    {
        public WhileNode(CommonTree tree) : base(tree)
        {
            ActualType = Type.While;
        }
    }
}
