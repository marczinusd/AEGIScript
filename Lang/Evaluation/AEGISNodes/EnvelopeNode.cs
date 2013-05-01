using System;
using System.Linq;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class EnvelopeNode : TermNode
    {
        public EnvelopeNode(Envelope envelope)
        {
            Value = envelope;
            ActualType = Type.Envelope;
        }

        public Envelope Value { get; set; }

        public override TermNode CallFun(FunCallNode func)
        {
            Type[] actualTypes = func.ResolvedArgs.Select(x => x.ActualType).ToArray();
            switch (func.FunName)
            {
                case "Center":
                    return Call(func, Center);
                case "Contains":
                    if (func.ResolvedArgs.Count == 1)
                    {
                        if (func.ResolvedArgs[0].ActualType == Type.Coordinate)
                        {
                            return Contains(func.ResolvedArgs[0] as CoordinateNode);
                        }
                        if (func.ResolvedArgs[0].ActualType == Type.Envelope)
                        {
                            return Contains(func.ResolvedArgs[0] as EnvelopeNode);
                        }
                    }
                    throw new Exception();
                case "Crosses":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Crosses);
                case "Disjoint":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Disjoint);
                case "Expand":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Expand);
                case "Distance":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Distance);
                case "Overlaps":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Overlaps);
                case "Touches":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Touches);
                case "Within":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Within);
                case "Intersects":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Intersects);
                case "Maximum":
                    return Call(func, Maximum);
                case "MaxZ":
                    return Call(func, MaxZ);
                case "MaxX":
                    return Call(func, MaxX);
                case "MaxY":
                    return Call(func, MaxY);
                case "Minimum":
                    return Call(func, Minimum);
                case "MinX":
                    return Call(func, MinX);
                case "MinY":
                    return Call(func, MinY);
                case "MinZ":
                    return Call(func, MinZ);
                case "IsValid":
                    return Call(func, IsValid);
                case "IsPlanar":
                    return Call(func, IsPlanar);
                case "IsEmpty":
                    return Call(func, IsEmpty);
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }


        private CoordinateNode Center()
        {
            return new CoordinateNode(Value.Center);
        }

        private BooleanNode Contains(CoordinateNode other)
        {
            return new BooleanNode(Value.Contains(other.Value));
        }

        private BooleanNode Contains(EnvelopeNode other)
        {
            return new BooleanNode(Value.Contains(other.Value));
        }

        private BooleanNode Crosses(EnvelopeNode other)
        {
            return new BooleanNode(Value.Crosses(other.Value));
        }

        private BooleanNode Disjoint(EnvelopeNode other)
        {
            return new BooleanNode(Value.Disjoint(other.Value));
        }

        private DoubleNode Distance(EnvelopeNode other)
        {
            return new DoubleNode(Value.Distance(other.Value));
        }

        private EnvelopeNode Expand(CoordinateNode other)
        {
            Value.Expand(other.Value);
            return this;
        }

        private BooleanNode Intersects(EnvelopeNode other)
        {
            return new BooleanNode(Value.Intersects(other.Value));
        }

        private BooleanNode Overlaps(EnvelopeNode other)
        {
            return new BooleanNode(Value.Overlaps(other.Value));
        }

        private BooleanNode Touches(EnvelopeNode other)
        {
            return new BooleanNode(Value.Touches(other.Value));
        }

        private BooleanNode Within(EnvelopeNode other)
        {
            return new BooleanNode(Value.Within(other.Value));
        }

        private BooleanNode IsValid()
        {
            return new BooleanNode(Value.IsValid);
        }

        private BooleanNode IsEmpty()
        {
            return new BooleanNode(Value.IsEmpty);
        }

        private BooleanNode IsPlanar()
        {
            return new BooleanNode(Value.IsPlanar);
        }

        private CoordinateNode Maximum()
        {
            return new CoordinateNode(Value.Maximum);
        }

        private CoordinateNode Minimum()
        {
            return new CoordinateNode(Value.Minimum);
        }

        private DoubleNode MaxX()
        {
            return new DoubleNode(Value.MaxX);
        }

        private DoubleNode MaxY()
        {
            return new DoubleNode(Value.MaxY);
        }

        private DoubleNode MaxZ()
        {
            return new DoubleNode(Value.MaxZ);
        }

        private DoubleNode MinX()
        {
            return new DoubleNode(Value.MinX);
        }

        private DoubleNode MinY()
        {
            return new DoubleNode(Value.MinY);
        }

        private DoubleNode MinZ()
        {
            return new DoubleNode(Value.MinZ);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}