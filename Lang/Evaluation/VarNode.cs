using Antlr.Runtime.Tree;
using System;


namespace AEGIScript.Lang.Evaluation
{
    class VarNode : TermNode
    {
        public VarNode(CommonTree tree, String sym, bool left = false) : base(tree, sym)
        {
            Symbol = sym;
        }

        public Type Interpret(AEGIScript.Lang.Scoping.Scope symbols)
        {
            //TODO
            return Type.VAR;
        }

        public string Symbol;
    }

    class IntVarNode : VarNode
    {
        public IntVarNode(CommonTree tree, String sym, bool left = false) : base(tree, sym, left)
        {

        }

        new public IntNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as IntNode;
        }
    }

    class DoubleVarNode : VarNode
    {
        public DoubleVarNode(CommonTree tree, String sym, bool left = false) 
            : base(tree, sym, left)
        {

        }

        new public DoubleNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as DoubleNode;
        }
    }

    class StringVarNode : VarNode
    {
        public StringVarNode(CommonTree tree, String sym, bool left = false)
            : base(tree, sym, left)
        {

        }

        new public StringNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as StringNode;
        }
    }

    class BoolVarNode : VarNode
    {
        public BoolVarNode(CommonTree tree, String sym, bool left = false)
            : base(tree, sym, left)
        {

        }

        new public BooleanNode Interpret(Scoping.Scope symbols)
        {
            return symbols.GetVar(Symbol) as BooleanNode;
        }
    }

}
