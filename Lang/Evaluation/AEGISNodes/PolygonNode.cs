using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class PolygonNode : SurfaceNode
    {
        public PolygonNode(Polygon poly) : base(poly)
        {
            ActualType = Type.Polygon;
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