using Antlr.Runtime.Tree;
using System;

namespace AEGIScript.Lang.Evaluation
{
    class BooleanNode : TermNode
    {
        public BooleanNode(CommonTree tree, string content)
            : base(tree, content)
        {
            Value = Boolean.Parse(content);
            ActualType = Type.Bool;
        }

        public BooleanNode(Boolean value)
        {
            Value = value;
            ActualType = Type.Bool;
        }


        public Boolean Interpret(SymbolTables.SymbolTable symbols)
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
        public NegationNode(CommonTree tree, string content)
            : base(tree, content)
        {
            ActualType = Type.Negation;
        }
    }
}
