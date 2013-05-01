using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class LineNode : LineStringNode
    {
        public LineNode(Line line) : base(line)
        {
            ActualType = Type.Line;
        }
    }
}