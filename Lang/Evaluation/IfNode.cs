using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AEGIScript.Lang.Evaluation
{
    class IfNode : ASTNode
    {
        public IfNode(CommonTree tree, string content) : base(tree)
        {
            this.ActualType = Type.IF;

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
        bool Cond;
    }

    class ElsifNode : ASTNode
    {
        public ElsifNode(CommonTree tree) : base(tree)
        {
            this.ActualType = Type.ELIF;
        }
    }

    class ElseNode : ASTNode
    {
        public ElseNode(CommonTree tree) : base(tree)
        {
            this.ActualType = Type.ELSE;
        }
    }
}
