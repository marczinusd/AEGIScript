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
                    node = new IntNode(tree, content) {ActualType = ASTNode.Type.INT};
                    return node;

                case "FLOAT":
                    node = new DoubleNode(tree, content) {ActualType = ASTNode.Type.DOUBLE, dispose = false};
                    return node;

                case "STRING":
                    node = new StringNode(tree, content) {ActualType = ASTNode.Type.STRING, dispose = false};
                    return node;

                case "BOOL":
                    node = new BooleanNode(tree, content) {ActualType = ASTNode.Type.BOOL, dispose = false};
                    return node;

                case "'while'":
                    node = new WhileNode(tree) {ActualType = ASTNode.Type.WHILE};
                    return node;

                case "'if'":
                    node = new IfNode(tree, content) {ActualType = ASTNode.Type.IF};
                    return node;
                case "'elsif'":
                    node = new ElsifNode(tree) {ActualType = ASTNode.Type.ELIF};
                    return node;
                case "'else'":
                    node = new ElseNode(tree) {ActualType = ASTNode.Type.ELSE};
                    return node;
                case "FUNC":
                    node = new FunCallNode(tree) {ActualType = ASTNode.Type.FUNCALL};
                    return node;
                case "ARRAY":
                    node = new ArrayNode(tree, content) {ActualType = ASTNode.Type.ARRAY};
                    return node;
                case "ACCESS":
                    node = new ArrAccessNode(tree, (tree.Children[0] as CommonTree).Text)
                        {
                            ActualType = ASTNode.Type.ARRACC
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
                    node = new ArithmeticNode(tree, tree.Text) {ActualType = ASTNode.Type.ARITH, dispose = false};
                    return node;

                case "'='":
                    node = new AssignNode(tree) {ActualType = ASTNode.Type.ASSIGN, dispose = false};
                    return node;

                case "IDENT":
                    // if the current node is the first child of its parent
                    var leftSide = tree.Parent.GetChild(0) as CommonTree == tree;
                    node = new VarNode(tree, tree.Text, leftSide) {ActualType = ASTNode.Type.VAR, dispose = false};
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