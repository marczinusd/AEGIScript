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
<<<<<<< HEAD:Lang/Evaluation/BooleanNode.cs
        public Boolean Interpret(SymbolTables.SymbolTable symbols)
=======
        public Boolean Interpret(SymbolTable symbols)
>>>>>>> Project restructured, geofactory support added:Lang/Evaluation/PrimitiveNodes/BooleanNode.cs
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
