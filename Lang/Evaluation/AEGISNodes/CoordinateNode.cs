using System;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class CoordinateNode : TermNode
    {
        public CoordinateNode(Coordinate coord)
        {
            Value = coord;
            ActualType = Type.Coordinate;
        }

        public Coordinate Value { get; set; }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "IsValid":
                    return Call(func, IsValid);
                case "IsEmpty":
                    return Call(func, IsEmpty);
                case "X":
                    return Call(func, X);
                case "Y":
                    return Call(func, Y);
                case "Z":
                    return Call(func, Z);
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }

        private DoubleNode X()
        {
            return new DoubleNode(Value.X);
        }

        private DoubleNode Y()
        {
            return new DoubleNode(Value.Y);
        }

        private DoubleNode Z()
        {
            return new DoubleNode(Value.Z);
        }

        private BooleanNode IsValid()
        {
            return new BooleanNode(Value.IsValid);
        }

        private BooleanNode IsEmpty()
        {
            return new BooleanNode(Value.IsEmpty);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}