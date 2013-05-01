using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class LinearRingNode : LineStringNode
    {
        public LinearRingNode(LinearRing ring) : base(ring)
        {
            ActualType = Type.LinearRing;
        }
    }
}