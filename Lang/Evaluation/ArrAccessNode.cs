using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;

namespace AEGIScript.Lang.Evaluation
{
    class ArrAccessNode : VarNode
    {
        public ArrAccessNode(CommonTree tree, String symbol)
            : base(tree, symbol)
        {
            Indices = new List<int>();
        }

        public List<Int32> Indices { get; set; }
    }
}
