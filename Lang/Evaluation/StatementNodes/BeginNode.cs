using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using AEGIScript.Lang.Scoping;
using Antlr.Runtime.Tree;
using System;
using System.Text;

namespace AEGIScript.Lang.Evaluation.StatementNodes
{
    class BeginNode : ASTNode
    {
        public BeginNode(CommonTree tree) : base(tree)
        {
            OutputBuilder = new StringBuilder();
        }

        public String Interpret(SymbolTable symbols)
        {
            return "";
        }

        public StringBuilder OutputBuilder { get; private set; }
    }
}
