using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class GeometryDimNode : TermNode
    {
        public GeometryDimNode(GeometryDimension dim)
        {
            Value = dim;
            ActualType = Type.Geometrydim;
        }

        public GeometryDimension Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}