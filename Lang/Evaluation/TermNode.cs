using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;


namespace AEGIScript.Lang.Evaluation
{
    class TermNode : ASTNode
    {
        public TermNode(CommonTree tree, String content) : base(tree)
        {

        }

        public TermNode()
        {

        }
    }
}
