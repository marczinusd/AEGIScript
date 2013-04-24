using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Evaluation
{
    public static class ASTNodeFactory
    {
        /// <summary>
        /// Factory for creating ASTNodes
        /// </summary>
        /// <param name="Tree">Tree of AST</param>
        /// <param name="TokenType">Actual token type</param>
        /// <param name="Content">Token text</param>
        /// <returns>Correctly typed AST node as ASTNode</returns>
        public static ASTNode CreateNode(CommonTree Tree, String TokenType, String Content)
        {
            ASTNode node;
            switch (TokenType)
            {
                case "INTEGER":
                    node = new IntNode(Tree, Content);
                    node.ActualType = ASTNode.Type.INT;
                    return node;

                case "FLOAT":
                    node = new DoubleNode(Tree, Content);
                    node.ActualType = ASTNode.Type.DOUBLE;
                    node.dispose = false;
                    return node;

                case "STRING":
                    node = new StringNode(Tree, Content);
                    node.ActualType = ASTNode.Type.STRING;
                    node.dispose = false;
                    return node;

                case "BOOL":
                    node = new BooleanNode(Tree, Content);
                    node.ActualType = ASTNode.Type.BOOL;
                    node.dispose = false;
                    return node;

                case "'while'":
                    node = new WhileNode(Tree);
                    node.ActualType = ASTNode.Type.WHILE;
                    return node;

                case "'if'":
                    node = new IfNode(Tree, Content);
                    node.ActualType = ASTNode.Type.IF;
                    return node;
                case "'elsif'":
                    node = new ElsifNode(Tree);
                    node.ActualType = ASTNode.Type.ELIF;
                    return node;
                case "'else'":
                    node = new ElseNode(Tree);
                    node.ActualType = ASTNode.Type.ELSE;
                    return node;
                case "FUNC":
                    node = new FunCallNode(Tree);
                    node.ActualType = ASTNode.Type.FUNCALL;
                    return node;
                case "ARRAY":
                    node = new ArrayNode(Tree, Content);
                    node.ActualType = ASTNode.Type.ARRAY;
                    return node;
                case "ACCESS":
                    node = new ArrAccessNode(Tree, (Tree.Children[0] as CommonTree).Text);
                    node.ActualType = ASTNode.Type.ARRACC;
                    return node;


                // we don't care specifically what kind of operator
                // we're dealing with -- at this point
                case "'||'": case "'&&'": case "'!='": case "'=='": case "'>='": case "'>'": case "'<='": case "'<'":
                case "'+'": case "'-'": case "'*'": case "'/'": case "'%'": case "'mod'":
                    node = new ArithmeticNode(Tree, Tree.Text);
                    node.ActualType = ASTNode.Type.ARITH;
                    node.dispose = false;
                    return node;

                case "'='":
                    node = new AssignNode(Tree);
                    node.ActualType = ASTNode.Type.ASSIGN;
                    node.dispose = false;
                    return node;

                case "IDENT":
                    // if the current node is the first child of its parent
                    bool leftSide = Tree.Parent.GetChild(0) as CommonTree == Tree;
                    node = new VarNode(Tree, Tree.Text, leftSide);
                    node.ActualType = ASTNode.Type.VAR;
                    node.dispose = false;
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
