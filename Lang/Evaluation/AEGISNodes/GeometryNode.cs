using System;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using Antlr.Runtime.Tree;
using ELTE.AEGIS.Core;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class GeometryNode : TermNode
    {
        public GeometryNode(CommonTree tree)
            : base(tree)
        {
            ActualType = Type.Geometry;
        }

        public GeometryNode(IGeometry geom)
        {
            Value = geom;
            Value.GeometryChanged += Value_GeometryChanged;
            ActualType = Type.Geometry;
        }

        public IGeometry Value { get; set; }

        private void Value_GeometryChanged(object sender, EventArgs e)
        {
            // todo?
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Boundary":
                    return Call(func, Boundary);
                case "ConvexHull":
                    return Call(func, ConvexHull);
                case "Envelope":
                    return Call(func, Envelope);
                case "Centroid":
                    return Call(func, Centroid);
                case "Clone":
                    return Call(func, Clone);
                case "Dimension":
                    return Call(func, Dimension);
                case "DimensionType":
                    return Call(func, DimensionType);
                case "Name":
                    return Call(func, Name);
                case "ReferenceSystem":
                    return Call(func, ReferenceSystem);
                case "IsValid":
                    return Call(func, IsValid);
                case "IsSimple":
                    return Call(func, IsSimple);
                case "IsEmpty":
                    return Call(func, IsEmpty);
                default:
                    return base.CallFun(func);
            }
        }

        private GeometryNode Boundary()
        {
            return new GeometryNode(Value.Boundary);
        }

        private GeometryNode ConvexHull()
        {
            return new GeometryNode(Value.ConvexHull);
        }

        private EnvelopeNode Envelope()
        {
            return new EnvelopeNode(Value.Envelope);
        }

        private CoordinateNode Centroid()
        {
            return new CoordinateNode(Value.Centroid);
        }

        private GeometryNode Clone()
        {
            return new GeometryNode(Value.Clone());
        }

        private IntNode Dimension()
        {
            return new IntNode(Value.Dimension);
        }

        private GeometryDimNode DimensionType()
        {
            return new GeometryDimNode(Value.DimensionType);
        }

        private StringNode Name()
        {
            return new StringNode(Value.Name);
        }

        private BooleanNode IsValid()
        {
            return new BooleanNode(Value.IsValid);
        }

        private BooleanNode IsSimple()
        {
            return new BooleanNode(Value.IsSimple);
        }

        private BooleanNode IsEmpty()
        {
            return new BooleanNode(Value.IsEmpty);
        }

        private ReferenceSystemNode ReferenceSystem()
        {
            return new ReferenceSystemNode(Value.ReferenceSystem);
        }


        public override string ToString()
        {
            return Value.ToString();
        }
    }
}