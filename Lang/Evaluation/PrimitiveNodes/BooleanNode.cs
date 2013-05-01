using AEGIScript.Lang.Scoping;
using Antlr.Runtime.Tree;
using System;

namespace AEGIScript.Lang.Evaluation.PrimitiveNodes
{
    class BooleanNode : TermNode
    {
        public BooleanNode(CommonTree tree, string content)
            : base(tree)
        {
            Value = Boolean.Parse(content);
            ActualType = Type.Bool;
        }

        public BooleanNode(Boolean value)
        {
            Value = value;
            ActualType = Type.Bool;
        }

        // todo: FIX ME
<<<<<<< HEAD:Lang/Evaluation/PrimitiveNodes/BooleanNode.cs
        public Boolean Interpret(SymbolTable symbols)
=======
        public Boolean Interpret(SymbolTables.SymbolTable symbols)
>>>>>>> 02d2e234ae3a1038fef2923d05ff58208dfe66a6:Lang/Evaluation/BooleanNode.cs
        {
            return true;
        }

        public Boolean Value { get; private set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    class NegationNode : TermNode
    {
        public NegationNode(CommonTree tree)
            : base(tree)
        {
            ActualType = Type.Negation;
        }
    }
}
