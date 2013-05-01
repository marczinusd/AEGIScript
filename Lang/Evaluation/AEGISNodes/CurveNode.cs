using System.Linq;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class CurveNode : GeometryNode
    {
        public CurveNode(Curve curve) : base(curve)
        {
            ActualType = Type.Curve;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "IsClosed":
                    return Call(func, IsClosed);
                case "IsRing":
                    return Call(func, IsRing);
                case "Count":
                    return Call(func, Count);
                case "Length":
                    return Call(func, Length);
                case "Coordinates":
                    return Call(func, Coordinates);
                case "StartCoordinate":
                    return Call(func, StartCoordinate);
                case "EndCoordinate":
                    return Call(func, EndCoordinate);

                default:
                    return base.CallFun(func);
            }
        }

        private BooleanNode IsClosed()
        {
            return new BooleanNode(((Curve) Value).IsClosed);
        }

        private BooleanNode IsRing()
        {
            return new BooleanNode(((Curve) Value).IsRing);
        }

        private IntNode Count()
        {
            return new IntNode(((Curve) Value).Count);
        }

        private DoubleNode Length()
        {
            return new DoubleNode(((Curve) Value).Length);
        }

        private ArrayNode Coordinates()
        {
            var curve = Value as Curve;
            var coords = curve.Coordinates.Select(t => new CoordinateNode(t)).Cast<TermNode>().ToList();
            return new ArrayNode(coords);
        }

        private CoordinateNode StartCoordinate()
        {
            return new CoordinateNode(((Curve) Value).StartCoordinate);
        }

        private CoordinateNode EndCoordinate()
        {
            return new CoordinateNode(((Curve) Value).EndCoordinate);
        }
    }
}