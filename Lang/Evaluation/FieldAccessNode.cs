using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation
{
    class FieldAccessNode : TermNode
    {
        public FieldAccessNode(CommonTree tree, String content) :base(tree, content)
        {
            
        }

        public TermNode Value { get; set; }
    }
}
