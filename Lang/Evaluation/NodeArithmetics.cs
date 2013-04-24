using System;
using AEGIScript.Lang.Exceptions;
using System.Text;

namespace AEGIScript.Lang.Evaluation
{

    /// <summary>
    /// Ugly singleton for arithmetic operations on nodes
    /// </summary>
    static class NodeArithmetics
    {
        #region Generic operation function
        #region Warning: MASSIVELY NON-DRY FUNCTION
        public static TermNode Op(TermNode left, TermNode right, ArithmeticNode.Operator op)
        {
            switch (left.ActualType)
            {
                case ASTNode.Type.INT:
                    switch(right.ActualType)
                    {
                        case ASTNode.Type.INT:
                            return Op(left as IntNode, right as IntNode, op);
                        case ASTNode.Type.STRING:
                            return Op(left as IntNode, right as StringNode, op);
                        case ASTNode.Type.BOOL:
                            return Op(left as IntNode, right as BooleanNode, op);
                        case ASTNode.Type.DOUBLE:
                            return Op(left as IntNode, right as DoubleNode, op);
                        default:
                            throw new Exception(BuildExMessage(left, right, op.ToString()));
                    }
                case ASTNode.Type.STRING:
                    switch (right.ActualType)
                    {
                        case ASTNode.Type.INT:
                            return Op(left as StringNode, right as IntNode, op);
                        case ASTNode.Type.STRING:
                            return Op(left as StringNode, right as StringNode, op);
                        case ASTNode.Type.BOOL:
                            return Op(left as StringNode, right as BooleanNode, op);
                        case ASTNode.Type.DOUBLE:
                            return Op(left as StringNode, right as DoubleNode, op);
                        default:
                            throw new Exception(BuildExMessage(left, right, op.ToString()));
                    }
                case ASTNode.Type.BOOL:
                    if (right.ActualType == ASTNode.Type.BOOL)
                    {
                        return Op(left as BooleanNode, right as BooleanNode, op);
                    }
                    else throw new Exception(BuildExMessage(left, right, op.ToString()));
                case ASTNode.Type.DOUBLE:
                    switch (right.ActualType)
                    {
                        case ASTNode.Type.INT:
                            return Op(left as DoubleNode, right as IntNode, op);
                        case ASTNode.Type.STRING:
                            return Op(left as DoubleNode, right as StringNode, op);
                        case ASTNode.Type.BOOL:
                            return Op(left as DoubleNode, right as BooleanNode, op);
                        case ASTNode.Type.DOUBLE:
                            return Op(left as DoubleNode, right as DoubleNode, op);
                        default:
                            throw new Exception(BuildExMessage(left, right, op.ToString()));
                    }
                case ASTNode.Type.ARRAY:
                    return Op(left as ArrayNode, right, op);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }
        #endregion
        #endregion

        public static TermNode Op(DoubleNode d_node, IntNode i_node, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new DoubleNode(d_node.Value + i_node.Value);
                case ArithmeticNode.Operator.MULT:
                    return new DoubleNode(d_node.Value * i_node.Value);
                case ArithmeticNode.Operator.DIV:
                    return new DoubleNode(d_node.Value / i_node.Value);
                case ArithmeticNode.Operator.MOD:
                    return new DoubleNode(d_node.Value % i_node.Value);
                case ArithmeticNode.Operator.MIN:
                    return new DoubleNode(d_node.Value - i_node.Value);
                case ArithmeticNode.Operator.EQ:
                    return new BooleanNode(d_node.Value == i_node.Value);
                case ArithmeticNode.Operator.NEQ:
                    return new BooleanNode(d_node.Value != i_node.Value);
                case ArithmeticNode.Operator.GEQ:
                    return new BooleanNode(d_node.Value >= i_node.Value);
                case ArithmeticNode.Operator.LEQ:
                    return new BooleanNode(d_node.Value <= i_node.Value);
                case ArithmeticNode.Operator.LT:
                    return new BooleanNode(d_node.Value < i_node.Value);
                case ArithmeticNode.Operator.GT:
                    return new BooleanNode(d_node.Value > i_node.Value);
                default:
                    throw new Exception(BuildExMessage(d_node, i_node, op.ToString()));
            }
        }

        public static TermNode Op(IntNode left, IntNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new IntNode(left.Value + right.Value);
                case ArithmeticNode.Operator.MULT:
                    return new IntNode(left.Value * right.Value);
                case ArithmeticNode.Operator.DIV:
                    return new IntNode(left.Value / right.Value);
                case ArithmeticNode.Operator.MOD:
                    return new IntNode(left.Value % right.Value);
                case ArithmeticNode.Operator.MIN:
                    return new IntNode(left.Value - right.Value);
                case ArithmeticNode.Operator.EQ:
                    return new BooleanNode(left.Value == right.Value);
                case ArithmeticNode.Operator.NEQ:
                    return new BooleanNode(left.Value != right.Value);
                case ArithmeticNode.Operator.GEQ:
                    return new BooleanNode(left.Value >= right.Value);
                case ArithmeticNode.Operator.LEQ:
                    return new BooleanNode(left.Value <= right.Value);
                case ArithmeticNode.Operator.LT:
                    return new BooleanNode(left.Value < right.Value);
                case ArithmeticNode.Operator.GT:
                    return new BooleanNode(left.Value > right.Value);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }

        public static TermNode Op(IntNode left, DoubleNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new DoubleNode(left.Value + right.Value);
                case ArithmeticNode.Operator.MULT:
                    return new DoubleNode(left.Value * right.Value);
                case ArithmeticNode.Operator.DIV:
                    return new DoubleNode(left.Value / right.Value);
                case ArithmeticNode.Operator.MOD:
                    return new DoubleNode(left.Value % right.Value);
                case ArithmeticNode.Operator.MIN:
                    return new DoubleNode(left.Value - right.Value);
                case ArithmeticNode.Operator.EQ:
                    return new BooleanNode(left.Value == right.Value);
                case ArithmeticNode.Operator.NEQ:
                    return new BooleanNode(left.Value != right.Value);
                case ArithmeticNode.Operator.GEQ:
                    return new BooleanNode(left.Value >= right.Value);
                case ArithmeticNode.Operator.LEQ:
                    return new BooleanNode(left.Value <= right.Value);
                case ArithmeticNode.Operator.LT:
                    return new BooleanNode(left.Value < right.Value);
                case ArithmeticNode.Operator.GT:
                    return new BooleanNode(left.Value > right.Value);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }


        public static TermNode Op(BooleanNode left, BooleanNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.EQ:
                    return new BooleanNode(left.Value == right.Value);
                case ArithmeticNode.Operator.NEQ:
                    return new BooleanNode(left.Value != right.Value);
                case ArithmeticNode.Operator.OR:
                    return new BooleanNode(left.Value || right.Value);
                case ArithmeticNode.Operator.AND:
                    return new BooleanNode(left.Value && right.Value);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }

        public static TermNode Op(DoubleNode left, DoubleNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new DoubleNode(left.Value + right.Value);
                case ArithmeticNode.Operator.MULT:
                    return new DoubleNode(left.Value * right.Value);
                case ArithmeticNode.Operator.DIV:
                    return new DoubleNode(left.Value / right.Value);
                case ArithmeticNode.Operator.MOD:
                    return new DoubleNode(left.Value % right.Value);
                case ArithmeticNode.Operator.MIN:
                    return new DoubleNode(left.Value - right.Value);
                case ArithmeticNode.Operator.EQ:
                    return new BooleanNode(left.Value == right.Value);
                case ArithmeticNode.Operator.NEQ:
                    return new BooleanNode(left.Value != right.Value);
                case ArithmeticNode.Operator.GEQ:
                    return new BooleanNode(left.Value >= right.Value);
                case ArithmeticNode.Operator.LEQ:
                    return new BooleanNode(left.Value <= right.Value);
                case ArithmeticNode.Operator.LT:
                    return new BooleanNode(left.Value < right.Value);
                case ArithmeticNode.Operator.GT:
                    return new BooleanNode(left.Value > right.Value);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }

        public static TermNode Op(ArrayNode left, TermNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return left.Add(right);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }

        public static TermNode Op(StringNode left, StringNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(left.Value + right.Value);
                case ArithmeticNode.Operator.EQ:
                    return new BooleanNode(left.Value == right.Value);
                case ArithmeticNode.Operator.NEQ:
                    return new BooleanNode(left.Value != right.Value);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }

        public static StringNode Op(StringNode s_node, IntNode i_node, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(s_node.Value + i_node.Value);
                default:
                    throw new Exception(BuildExMessage(s_node, i_node, op.ToString()));
            }
        }

        public static StringNode Op(IntNode i_node, StringNode s_node, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(i_node.Value + s_node.Value);
                default:
                    throw new Exception(BuildExMessage(i_node, s_node, op.ToString()));
            }
        }

        public static StringNode Op(StringNode s_node, DoubleNode d_node, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(s_node.Value + d_node.Value);
                default:
                    throw new Exception(BuildExMessage(s_node, d_node, op.ToString()));
            }
        }

        public static StringNode Op(DoubleNode d_node, StringNode s_node, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    //return new StringNode(d_node.Value + s_node.Value);
                    return new StringNode(d_node.Value.ToString() + s_node.Value);
                default:
                    throw new Exception(BuildExMessage(d_node, s_node, op.ToString()));
            }
        }

        public static StringNode Op(StringNode s_node, BooleanNode b_node, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(s_node.Value + b_node.Value);

                default:
                    throw new Exception(BuildExMessage(s_node, b_node, op.ToString()));
            }
        }

        public static StringNode Op(BooleanNode b_node, StringNode s_node, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(b_node.Value + s_node.Value);

                default:
                    throw new Exception(BuildExMessage(b_node, s_node, op.ToString()));
            }
        }

        public static string BuildExMessage(TermNode leftType, TermNode rightType, string operation)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("RUNTIME ERROR: \n Invalid operation performed! ");
            builder.Append("Types: " + leftType.ActualType.ToString() + " and " + rightType.ActualType.ToString() + " do not define operation: " + operation + "\n");
            if (leftType.Line != null)
            {
                builder.AppendLine("Error occured at line: " + leftType.Line);
            }
            else if (rightType.Line != null)
            {
                builder.AppendLine("Error occured at line: " + rightType.Line);
            }
            return builder.ToString();
        }
    }
}
