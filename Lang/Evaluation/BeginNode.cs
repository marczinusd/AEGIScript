using Antlr.Runtime.Tree;
using System;
using System.Text;

namespace AEGIScript.Lang.Evaluation
{
    class BeginNode : ASTNode
    {
        public BeginNode(CommonTree tree) : base(tree)
        {
            OutputBuilder = new StringBuilder();
        }

        public String Interpret(SymbolTables.SymbolTable symbols)
        {
            return "";
        }

        public StringBuilder OutputBuilder { get; private set; }
    }
}
