﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;
using System.Windows;

namespace AEGIScript.Lang.Evaluation
{
    public class ASTNode
    {
        public enum Type
        {
            ARITH,
            WHILE,
            IF,
            ELIF,
            ELSE,
            FUNCALL,
            ASSIGN,
            VAR,
            INTVAR,
            BOOLVAR,
            DOUBLEVAR,
            STRINGVAR,
            ARRAY,
            ARRACC,
            INTARRAY,
            DOUBLEARRAY,
            BOOLARRAY,
            STRINGARRAY,
            TERM,
            INT,
            STRING,
            BOOL,
            DOUBLE,
            PRINT,
        }


        // do not use!
        public ASTNode() { dispose = true;  }

        public ASTNode(bool _dispose) { dispose = _dispose; }

        /// <summary>
        /// Constructor for creating a node with no children
        /// </summary>
        /// <param name="_tree"></param>
        public ASTNode(CommonTree _tree)
        {
            dispose = false;
            Tree = _tree;
            Line = _tree.Line;
            SetTypedChildren();
        }

        public ASTNode(CommonTree _tree, String[] TokenTypes)
        {
            dispose = false;
            Line = _tree.Line;
            Tree = _tree;
            SetTypedChildren();
        }

        public void SetTypedChildren()
        {
            Children = new List<ASTNode>();
            for (int i = 0; Tree.Children != null && i < Tree.Children.Count; i++)
            {
                CommonTree tree = Tree.GetChild(i) as CommonTree;
                if (tree.ToString().Contains("mismatched token"))
                {
                    throw new Exception("Parse error! \n Mismatched token found with tree signature: \n" + tree.ToStringTree() + "\n");
                }
                ASTNode node = ASTNodeFactory.CreateNode(tree, TokenTypeMediator.GetTokenType(tree.Token.Type), tree.Text);
                node.Parent = this;
                if (!node.dispose)
                {
                    //MessageBox.Show(node.ActualType.ToString() + "\n" + TokenTypeMediator.GetTokenType(tree.Token.Type) + "\n" + tree.Text);
                    Children.Add(node);
                }
            }
        }

        /// <summary>
        /// Converts ANTLR's commontree to AST nodes -- warning: sets up the whole subtree!
        /// Use the constructor with 1 param to set up the single node!
        /// </summary>
        /// <param name="TokenTypes">Node children's types as string</param>
        /// should reimplement for ASTNode with enum containing actual type information
        public void SetTypedChildren(String[] TokenTypes)
        {
            Children = new List<ASTNode>();
            for (int i = 0; i < Tree.ChildCount; i++)
            {
                int TokenIndex = (Tree.GetChild(i) as CommonTree).Type;
                if (TokenIndex > 0)
                {
                    String TokenType = TokenTypes[TokenIndex];
                    CommonTree tree = Tree.GetChild(i) as CommonTree;
                    ASTNode node = ASTNodeFactory.CreateNode(tree, TokenType, tree.Text);
                    node.Parent = this;
                    Children.Add(node);
                }
            }
        }
        public Boolean dispose { get; set; }

        public ASTNode Parent { get; set; }

        public Type ActualType { get; set; }

        public Boolean Visited { get; set; }

        public List<ASTNode> Children { get; private set; }

        public virtual void Accept(IVisitor visitor) { }

        private IVisitor visitor { get; set; }

        public CommonTree Tree { get; private set; }

        public int Line { get; private set; }
    }
}
