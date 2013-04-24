using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;


namespace AEGIScript.Lang.Evaluation
{
    class FunCallNode : ASTNode
    {
        enum ActualFun
        {
            PRINT,
            READ
        }

        public FunCallNode(CommonTree tree) : base(tree)
        {
            this.ActualType = Type.FUNCALL;
            FunName = (tree.Children[0] as CommonTree).Text;
        }


        public TermNode ReturnValue { get; set; }
        public string FunName { get; private set; }
    }
}
