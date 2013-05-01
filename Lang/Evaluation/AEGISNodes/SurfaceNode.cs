using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class SurfaceNode : GeometryNode
    {
        public SurfaceNode(Surface surf) : base(surf)
        {
            ActualType = Type.Surface;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "IsConvex":
                    return Call(func, IsConvex);
                case "IsDivided":
                    return Call(func, IsDivided);
                case "IsWhole":
                    return Call(func, IsWhole);
                case "Area":
                    return Call(func, Area);
                case "Perimeter":
                    return Call(func, Perimeter);
                default:
                    return base.CallFun(func);
            }
        }

        public BooleanNode IsConvex()
        {
            return new BooleanNode(((Surface) Value).IsConvex);
        }

        public BooleanNode IsDivided()
        {
            return new BooleanNode(((Surface) Value).IsDivided);
        }

        public BooleanNode IsWhole()
        {
            return new BooleanNode(((Surface) Value).IsWhole);
        }

        public DoubleNode Area()
        {
            return new DoubleNode(((Surface) Value).Area);
        }

        public DoubleNode Perimeter()
        {
            return new DoubleNode(((Surface) Value).Perimeter);
        }
    }
}