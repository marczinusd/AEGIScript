using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;
using ELTE.AEGIS.Core;


namespace AEGIScript.Lang.Evaluation
{
    class TermNode : ASTNode
    {
        public TermNode(CommonTree tree, String content) : base(tree) { }

        public TermNode(CommonTree tree) { }

        public TermNode() { }


    }

// ReSharper disable InconsistentNaming
    class IGeometryNode : TermNode
// ReSharper restore InconsistentNaming
    {
        public IGeometryNode(CommonTree tree) : base(tree)
        {
            
        }

        public IGeometry Value { get; set; }
    }
}
