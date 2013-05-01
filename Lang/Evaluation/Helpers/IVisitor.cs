using AEGIScript.Lang.Evaluation.PrimitiveNodes;

namespace AEGIScript.Lang.Evaluation.Helpers
{
    public interface IVisitor
    {
        void Visit(ASTNode node);
    }
}
