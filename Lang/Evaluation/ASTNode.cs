using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;

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

            GEOMETRY,
            GEOMREADER,
            TIFFREADER,
            GEOTIFFREADER,
            SHAPEFREADER,

            ENVELOPE,
            COORDINATE,
            GEOMETRYDIM,
            METADATA,
            REFERENCESYS,

            DOUBLE,
            PRINT,
            FIELDACCESS,
        }


        // do not use!
        public ASTNode()
        {
            dispose = true;
        }

        public ASTNode(bool _dispose)
        {
            dispose = _dispose;
        }

        /// <summary>
        ///     Constructor for creating a node with no children
        /// </summary>
        /// <param name="tree"></param>
        public ASTNode(CommonTree tree)
        {
            dispose = false;
            Tree = tree;
            Line = tree.Line;
            SetTypedChildren();
        }

        public ASTNode(CommonTree tree, String[] tokenTypes)
        {
            dispose = false;
            Line = tree.Line;
            Tree = tree;
            SetTypedChildren();
        }

        public Boolean dispose { get; set; }

        public ASTNode Parent { get; set; }

        public Type ActualType { get; set; }

        public Boolean Visited { get; set; }

        public List<ASTNode> Children { get; private set; }

        private IVisitor visitor { get; set; }

        public CommonTree Tree { get; private set; }

        public int Line { get; private set; }

        public void SetTypedChildren()
        {
            Children = new List<ASTNode>();
            for (int i = 0; Tree.Children != null && i < Tree.Children.Count; i++)
            {
                var tree = Tree.GetChild(i) as CommonTree;
                if (tree.ToString().Contains("mismatched token"))
                {
                    throw new Exception("Parse error! \n Mismatched token found with tree signature: \n" +
                                        tree.ToStringTree() + "\n");
                }
                ASTNode node = ASTNodeFactory.CreateNode(tree, TokenTypeMediator.GetTokenType(tree.Token.Type),
                                                         tree.Text);
                node.Parent = this;
                if (!node.dispose)
                {
                    //MessageBox.Show(node.ActualType.ToString() + "\n" + TokenTypeMediator.GetTokenType(tree.Token.Type) + "\n" + tree.Text);
                    Children.Add(node);
                }
            }
        }

        /// <summary>
        ///     Converts ANTLR's commontree to AST nodes -- warning: sets up the whole subtree!
        ///     Use the constructor with 1 param to set up the single node!
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
                    var tree = Tree.GetChild(i) as CommonTree;
                    ASTNode node = ASTNodeFactory.CreateNode(tree, TokenType, tree.Text);
                    node.Parent = this;
                    Children.Add(node);
                }
            }
        }

        public virtual void Accept(IVisitor visitor)
        {
        }
    }
}