using Antlr.Runtime.Tree;
using System;


namespace AEGIScript.Lang.Evaluation
{
    class VarNode : TermNode
    {
        public VarNode(CommonTree tree, String sym) : base(tree)
        {
            Symbol = sym;
        }

        public Type Interpret(Scoping.Scope symbols)
        {
            //TODO
            return Type.Var;
        }

        public string Symbol;
    }

    class IntVarNode : VarNode
    {
        public IntVarNode(CommonTree tree, String sym) : base(tree, sym)
        {

        }

        new public IntNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as IntNode;
        }
    }

    class DoubleVarNode : VarNode
    {
        public DoubleVarNode(CommonTree tree, String sym) 
            : base(tree, sym)
        {

        }

        new public DoubleNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as DoubleNode;
        }
    }

    class StringVarNode : VarNode
    {
        public StringVarNode(CommonTree tree, String sym)
            : base(tree, sym)
        {

        }

        new public StringNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as StringNode;
        }
    }

    class BoolVarNode : VarNode
    {
        public BoolVarNode(CommonTree tree, String sym)
            : base(tree, sym)
        {

        }

        new public BooleanNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as BooleanNode;
        }
    }

}
