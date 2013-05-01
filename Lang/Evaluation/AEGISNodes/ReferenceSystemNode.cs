using System;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class ReferenceSystemNode : TermNode
    {
        public ReferenceSystemNode(IReferenceSystem referenceSystem)
        {
            Value = referenceSystem;
            ActualType = Type.ReferenceSys;
        }

        public ReferenceSystemNode()
        {
            Value = GeometryFactory.ReferenceSystem;
        }

        public IReferenceSystem Value { get; set; }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Dimension":
                    return Call(func, Dimension);
                case "Name":
                    return Call(func, Dimension);
                case "Identifier":
                    return Call(func, Identifier);
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }

        private IntNode Dimension()
        {
            return new IntNode(Value.Dimension);
        }

        private StringNode Name()
        {
            return new StringNode(Value.Name);
        }

        private StringNode Identifier()
        {
            return new StringNode(Value.Identifier);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}