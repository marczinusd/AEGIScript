using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class RectangleNode : PolygonNode
    {
        public RectangleNode(Rectangle rect) : base(rect)
        {
            ActualType = Type.Rectangle;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                default:
                    return base.CallFun(func);
            }
        }
    }
}