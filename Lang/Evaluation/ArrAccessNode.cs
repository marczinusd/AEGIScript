using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Evaluation
{
    class ArrAccessNode : VarNode
    {
        public ArrAccessNode(CommonTree tree, String symbol)
            : base(tree, symbol)
        {
            Indices = new List<int>();
            //for (int i = 1; i < Children.Count; i++)
            //{
            //    Indices.Add(Int32.Parse(Children[i].Tree.Text));
            //}
            //ActualType = ASTNode.Type.ARRACC;
        }

        public List<Int32> Indices { get; set; }
    }
}
