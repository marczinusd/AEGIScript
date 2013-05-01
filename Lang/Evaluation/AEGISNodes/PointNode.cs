using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class PointNode : GeometryNode
    {
        public PointNode(Point p) : base(p)
        {
            ActualType = Type.Point;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "X":
                    return Call(func, X);
                case "Y":
                    return Call(func, Y);
                case "Z":
                    return Call(func, Z);

                default:
                    return base.CallFun(func);
            }
        }


        private DoubleNode X()
        {
            return new DoubleNode((Value as Point).X);
        }

        private DoubleNode Y()
        {
            return new DoubleNode((Value as Point).Y);
        }

        private DoubleNode Z()
        {
            return new DoubleNode((Value as Point).Z);
        }

    }
}