using Antlr.Runtime.Tree;
using AEGIScript.GUI.Model;
using AEGIScript.Lang.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Evaluation
{
    class VarNode : TermNode
    {
        public VarNode(CommonTree tree, String sym, bool left = false) : base(tree, sym)
        {
            IsOnLeftSide = left;
            Symbol = sym;
        }

        public ASTNode.Type Interpret(AEGIScript.Lang.Scope.Scope symbols)
        {
            //TODO
            return Type.VAR;
        }

        public string Symbol;
        private bool IsOnLeftSide;
    }

    class IntVarNode : VarNode
    {
        public IntVarNode(CommonTree tree, String sym, bool left = false) : base(tree, sym, left)
        {

        }

        new public IntNode Interpret(AEGIScript.Lang.Scope.Scope symbols)
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

        new public DoubleNode Interpret(AEGIScript.Lang.Scope.Scope symbols)
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

        new public StringNode Interpret(AEGIScript.Lang.Scope.Scope symbols)
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

        new public BooleanNode Interpret(AEGIScript.Lang.Scope.Scope symbols)
        {
            return symbols.GetVar(Symbol) as BooleanNode;
        }
    }

}
