using System;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation
{
    public static class ASTNodeFactory
    {
        /// <summary>
        ///     Factory for creating ASTNodes
        /// </summary>
        /// <param name="tree">Tree of AST</param>
        /// <param name="tokenType">Actual token type</param>
        /// <param name="content">Token text</param>
        /// <returns>Correctly typed AST node as ASTNode</returns>
        public static ASTNode CreateNode(CommonTree tree, String tokenType, String content)
        {
            ASTNode node;
            switch (tokenType)
            {
                case "INTEGER":
                    node = new IntNode(tree, content) {ActualType = ASTNode.Type.Int};
                    return node;

                case "FLOAT":
                    node = new DoubleNode(tree, content) {ActualType = ASTNode.Type.Double, Dispose = false};
                    return node;

                case "STRING":
                    node = new StringNode(tree, content) {ActualType = ASTNode.Type.String, Dispose = false};
                    return node;


                //case "'NOT'":
                //    bool negated = true;
                //    while (tree.Children != null)
                //    {
                //        tree = tree.Children[0] as CommonTree;
                //        negated = negated ^ true;
                //    }
                //    node = new BooleanNode(tree, content) { ActualType = ASTNode.Type.Bool, dispose = false };
                case "BOOL":
                    node = new BooleanNode(tree, content) {ActualType = ASTNode.Type.Bool, Dispose = false};
                    return node;

                case "'while'":
                    node = new WhileNode(tree) {ActualType = ASTNode.Type.While};
                    return node;

                case "'if'":
                    node = new IfNode(tree, content) {ActualType = ASTNode.Type.If};
                    return node;
                case "'elsif'":
                    node = new ElsifNode(tree) {ActualType = ASTNode.Type.Elif};
                    return node;
                case "'else'":
                    node = new ElseNode(tree) {ActualType = ASTNode.Type.Else};
                    return node;
                case "FUNC":
                    node = new FunCallNode(tree) {ActualType = ASTNode.Type.FunCall};
                    return node;
                case "ARRAY":
                    node = new ArrayNode(tree, content) {ActualType = ASTNode.Type.Array};
                    return node;

                case "FIELD_ACCESS":
                    node = new FieldAccessNode(tree, content) { ActualType =  ASTNode.Type.FieldAccess };
                    return node;
                case "ACCESS":
                    node = new ArrAccessNode(tree, (tree.Children[0] as CommonTree).Text)
                        {
                            ActualType = ASTNode.Type.ArrAcc
                        };
                    return node;


                    // we don't care specifically what kind of operator
                    // we're dealing with -- at this point
                case "'||'":
                case "'&&'":
                case "'!='":
                case "'=='":
                case "'>='":
                case "'>'":
                case "'<='":
                case "'<'":
                case "'+'":
                case "'-'":
                case "'*'":
                case "'/'":
                case "'%'":
                case "'mod'":
                    if (tokenType == "'-'" && tree.Children.Count == 1)
                    {
                        node = new NegativeNode(tree, tree.Text) {ActualType = ASTNode.Type.Negative, Dispose = false};
                        return node;
                    }
                    node = new ArithmeticNode(tree, tree.Text) {ActualType = ASTNode.Type.Arith, Dispose = false};
                    return node;

                case "'not'":
                    node = new NegationNode(tree, tree.Text) {ActualType = ASTNode.Type.Negation, Dispose = false};
                    return node;

                case "'='":
                    node = new AssignNode(tree) {ActualType = ASTNode.Type.Assign, Dispose = false};
                    return node;

                case "IDENT":
                    // if the current node is the first child of its parent
                    var leftSide = tree.Parent.GetChild(0) as CommonTree == tree;
                    node = new VarNode(tree, tree.Text) {ActualType = ASTNode.Type.Var, Dispose = false};
                    return node;
                    // Uncomment not to drop certain nodes
                    //default:
                    //    node = new Node(new ASTNode(Tree), "OTHER");
                    //    break;
            }
            return new ASTNode(true);
        }
    }
}