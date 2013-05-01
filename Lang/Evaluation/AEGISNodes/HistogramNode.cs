using System.Linq;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry.Raster;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class HistogramNode : TermNode
    {
        public HistogramNode(Histogram value)
        {
            ActualType = Type.Histogram;
            Value = value;
        }

        public UInt32Node MinimalIntensity()
        {
            return new UInt32Node(Value.MinimalIntensity);
        }

        public UInt32Node MaximalIntensity()
        {
            return new UInt32Node(Value.MaximalIntensity);
        }

        public UInt32Node MaximalOccurence()
        {
            return new UInt32Node(Value.MaximalOccurrance);
        }

        public IntNode PixelCount()
        {
            return new IntNode(Value.PixelCount);
        }

        public DoubleNode MeanValue()
        {
            return new DoubleNode(Value.MeanValue);
        }

        public DoubleNode DeviationValue()
        {
            return new DoubleNode(Value.DeviationValue);
        }

        public IntNode RadiometricResolution()
        {
            return new IntNode(Value.RadiometricResolution);
        }

        public ArrayNode Values()
        {
            var nodeVals = Value.Values
                                .Select(val => new UInt32Node(val)).Cast<TermNode>().ToList();
            return new ArrayNode(nodeVals);
        }

        public ArrayNode CulcumativeDistributionValues()
        {
            var nodeVals = Value.CulcumativeDistibutionValues
                                .Select(val => new UInt32Node(val)).Cast<TermNode>().ToList();
            return new ArrayNode(nodeVals);
        }

        public DoubleNode OtsuThreshold()
        {
            return new DoubleNode(Value.OtsuThreshold);
        }

        public Histogram Value { get; private set; }
    }
}