using System.Linq;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class LineStringNode : CurveNode
    {
        public LineStringNode(LineString ls) : base(ls)
        {
            ActualType = Type.LineString;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            Type[] actualTypes = func.ResolvedArgs.Select(x => x.ActualType).ToArray();
            switch (func.FunName)
            {
                case "GetCoordinate":
                    return Call<IntNode>(new[] {Type.Int}, actualTypes, func, GetCoordinate);
                case "SetCoordinate":
                    return Call<IntNode, CoordinateNode>(new[] {Type.Int, Type.Coordinate}, actualTypes,
                                                         func, SetCoordinate);
                case "Includes":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Includes);
                case "Add":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Add);
                case "Insert":
                    return Call<IntNode, CoordinateNode>(new[] {Type.Int, Type.Coordinate}, actualTypes,
                                                         func, Insert);
                case "Remove":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Remove);
                case "RemoveAt":
                    return Call<IntNode>(new[] {Type.Int}, actualTypes, func, RemoveAt);
                case "Clear":
                    return Call(func, Clear);

                default:
                    return base.CallFun(func);
            }
        }

        private LineStringNode SetCoordinate(IntNode ind, CoordinateNode coord)
        {
            ((LineString) Value).SetCoordinate(ind.Value, coord.Value);
            return this;
        }

        private BooleanNode Includes(CoordinateNode coord)
        {
            return new BooleanNode(((LineString) Value).Includes(coord.Value));
        }

        private LineStringNode Add(CoordinateNode coord)
        {
            ((LineString) Value).Add(coord.Value);
            return this;
        }

        private LineStringNode Insert(IntNode ind, CoordinateNode coord)
        {
            ((LineString) Value).Insert(ind.Value, coord.Value);
            return this;
        }

        private LineStringNode Remove(CoordinateNode node)
        {
            ((LineString) Value).Remove(node.Value);
            return this;
        }

        private LineStringNode RemoveAt(IntNode ind)
        {
            ((LineString) Value).RemoveAt(ind.Value);
            return this;
        }

        private LineStringNode Clear()
        {
            ((LineString) Value).Clear();
            return this;
        }

        private CoordinateNode GetCoordinate(IntNode ind)
        {
            return new CoordinateNode(((LineString) Value).GetCoordinate(ind.Value));
        }
    }
}