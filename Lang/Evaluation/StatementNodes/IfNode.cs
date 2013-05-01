using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using Antlr.Runtime.Tree;
using System.Collections.Generic;

namespace AEGIScript.Lang.Evaluation.StatementNodes
{
    class IfNode : ASTNode
    {
        public IfNode(CommonTree tree) : base(tree)
        {
            ActualType = Type.If;

            Clauses = new List<ElsifNode>();
            foreach (ASTNode n in Children)
            {
                if (n is ElsifNode)
                {
                    Clauses.Add(n as ElsifNode);
                }
                if (n is ElseNode)
                {
                    Else = n as ElseNode;   
                }
            }
        }

        public List<ElsifNode> Clauses { get; set; }
        public ElseNode Else { get; set; }
    }

    class ElsifNode : ASTNode
    {
        public ElsifNode(CommonTree tree) : base(tree)
        {
            ActualType = Type.Elif;
        }
    }

    class ElseNode : ASTNode
    {
        public ElseNode(CommonTree tree) : base(tree)
        {
            ActualType = Type.Else;
        }
    }
}
