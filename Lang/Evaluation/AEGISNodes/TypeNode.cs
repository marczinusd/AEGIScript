using AEGIScript.Lang.Evaluation.PrimitiveNodes;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class TypeNode : TermNode
    {
        public TypeNode(Type nodeType)
        {
            ActualType = Type.TypeIdentifier;
            NodeType = nodeType;
        }

        public override string ToString()
        {
            return NodeType.ToString();
        }

        public Type NodeType { get; private set; }
    }
}