using System;
using System.Collections.Generic;
using AEGIScript.Lang.Evaluation.Helpers;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation.PrimitiveNodes
{
    public class ASTNode
    {
        public enum Type
        {
            Arith,
            While,
            If,
            Elif,
            Else,
            FunCall,
            Assign,
            Var,
            Boolvar,
            Array,
            ArrAcc,
            Term,
            FieldAccess,
            Negation,
            Negative,


            // primitive types
            Int,
            String,
            Bool,
            Double,

            // joker type for function calls
            Any,

            // AEGIS types
            Geometry,
            Geomreader,
            Tiffreader,
            Geotiffreader,
            Shapefreader,
            Envelope,
            Coordinate,
            Geometrydim,
            Metadata,
            ReferenceSys,
            LineString,
            Curve,
            Polygon,
            Surface,
            Rectangle,
            Raster,
            RasterBand,
            UInt16,
            UInt32,
            Histogram,
            TypeIdentifier,
            Point,
            Line,
            LinearRing
        }


        // do not use!
        public ASTNode()
        {
            Dispose = true;
        }

        public ASTNode(bool dispose)
        {
            Dispose = dispose;
        }

        /// <summary>
        ///     Constructor for creating a node with no children
        /// </summary>
        /// <param name="tree"></param>
        public ASTNode(CommonTree tree)
        {
            Dispose = false;
            Tree = tree;
            Line = tree.Line;
            SetTypedChildren();
        }

        public Boolean Dispose { get; set; }

        public ASTNode Parent { get; set; }

        public Type ActualType { get; set; }

        public Boolean Visited { get; set; }

        public List<ASTNode> Children { get; private set; }

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
                if (!node.Dispose)
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
        /// <param name="tokenTypes">Node children's types as string</param>
        /// should reimplement for ASTNode with enum containing actual type information
        public void SetTypedChildren(String[] tokenTypes)
        {
            Children = new List<ASTNode>();
            for (int i = 0; i < Tree.ChildCount; i++)
            {
                int tokenIndex = ((CommonTree) Tree.GetChild(i)).Type;
                if (tokenIndex > 0)
                {
                    var tokenType = tokenTypes[tokenIndex];
                    var tree = Tree.GetChild(i) as CommonTree;
                    var node = ASTNodeFactory.CreateNode(tree, tokenType, tree.Text);
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