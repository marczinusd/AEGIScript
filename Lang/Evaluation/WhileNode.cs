using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Evaluation
{
    class WhileNode : ASTNode
    {
        public WhileNode(CommonTree tree) : base(tree)
        {
            this.ActualType = Type.WHILE;
        }
    }
}
