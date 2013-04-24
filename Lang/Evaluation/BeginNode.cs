using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;

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
