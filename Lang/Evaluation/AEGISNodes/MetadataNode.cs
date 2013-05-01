using System.Collections.Generic;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class MetadataNode : TermNode
    {
        public MetadataNode(IDictionary<string, object> val)
        {
            Value = val;
            ActualType = Type.Metadata;
        }

        public IDictionary<string, object> Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}