using System;
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
                case ASTNode.Type.Int:
                    switch(right.ActualType)
                    {
                        case ASTNode.Type.Int:
                            return Op(left as IntNode, right as IntNode, op);
                        case ASTNode.Type.String:
                            return Op(left as IntNode, right as StringNode, op);
                        case ASTNode.Type.Bool:
                            return Op(left as IntNode, right as BooleanNode, op);
                        case ASTNode.Type.Double:
                            return Op(left as IntNode, right as DoubleNode, op);
                        default:
                            throw new Exception(BuildExMessage(left, right, op.ToString()));
                    }
                case ASTNode.Type.String:
                    switch (right.ActualType)
                    {
                        case ASTNode.Type.Int:
                            return Op(left as StringNode, right as IntNode, op);
                        case ASTNode.Type.String:
                            return Op(left as StringNode, right as StringNode, op);
                        case ASTNode.Type.Bool:
                            return Op(left as StringNode, right as BooleanNode, op);
                        case ASTNode.Type.Double:
                            return Op(left as StringNode, right as DoubleNode, op);
                        default:
                            throw new Exception(BuildExMessage(left, right, op.ToString()));
                    }
                case ASTNode.Type.Bool:
                    switch (right.ActualType)
                    {
                        case ASTNode.Type.Bool:
                            return Op(left as BooleanNode, right as BooleanNode, op);
                        default:
                            throw new Exception(BuildExMessage(left, right, op.ToString()));
                    }
                case ASTNode.Type.Double:
                    switch (right.ActualType)
                    {
                        case ASTNode.Type.Int:
                            return Op(left as DoubleNode, right as IntNode, op);
                        case ASTNode.Type.String:
                            return Op(left as DoubleNode, right as StringNode, op);
                        case ASTNode.Type.Bool:
                            return Op(left as DoubleNode, right as BooleanNode, op);
                        case ASTNode.Type.Double:
                            return Op(left as DoubleNode, right as DoubleNode, op);
                        default:
                            throw new Exception(BuildExMessage(left, right, op.ToString()));
                    }
                case ASTNode.Type.Array:
                    return Op(left as ArrayNode, right, op);
                default:
                    throw new Exception(BuildExMessage(left, right, op.ToString()));
            }
        }
        #endregion
        #endregion

        public static TermNode Op(DoubleNode dNode, IntNode iNode, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new DoubleNode(dNode.Value + iNode.Value);
                case ArithmeticNode.Operator.MULT:
                    return new DoubleNode(dNode.Value * iNode.Value);
                case ArithmeticNode.Operator.DIV:
                    return new DoubleNode(dNode.Value / iNode.Value);
                case ArithmeticNode.Operator.MOD:
                    return new DoubleNode(dNode.Value % iNode.Value);
                case ArithmeticNode.Operator.MIN:
                    return new DoubleNode(dNode.Value - iNode.Value);
                case ArithmeticNode.Operator.EQ:
                    return new BooleanNode(dNode.Value == iNode.Value);
                case ArithmeticNode.Operator.NEQ:
                    return new BooleanNode(dNode.Value != iNode.Value);
                case ArithmeticNode.Operator.GEQ:
                    return new BooleanNode(dNode.Value >= iNode.Value);
                case ArithmeticNode.Operator.LEQ:
                    return new BooleanNode(dNode.Value <= iNode.Value);
                case ArithmeticNode.Operator.LT:
                    return new BooleanNode(dNode.Value < iNode.Value);
                case ArithmeticNode.Operator.GT:
                    return new BooleanNode(dNode.Value > iNode.Value);
                default:
                    throw new Exception(BuildExMessage(dNode, iNode, op.ToString()));
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

        public static StringNode Op(StringNode sNode, IntNode iNode, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(sNode.Value + iNode.Value);
                default:
                    throw new Exception(BuildExMessage(sNode, iNode, op.ToString()));
            }
        }

        public static StringNode Op(IntNode iNode, StringNode sNode, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(iNode.Value + sNode.Value);
                default:
                    throw new Exception(BuildExMessage(iNode, sNode, op.ToString()));
            }
        }

        public static StringNode Op(StringNode sNode, DoubleNode dNode, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(sNode.Value + dNode.Value);
                default:
                    throw new Exception(BuildExMessage(sNode, dNode, op.ToString()));
            }
        }

        public static StringNode Op(DoubleNode dNode, StringNode sNode, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    //return new StringNode(d_node.Value + s_node.Value);
                    return new StringNode(dNode.Value + sNode.Value);
                default:
                    throw new Exception(BuildExMessage(dNode, sNode, op.ToString()));
            }
        }

        public static StringNode Op(StringNode sNode, BooleanNode bNode, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(sNode.Value + bNode.Value);

                default:
                    throw new Exception(BuildExMessage(sNode, bNode, op.ToString()));
            }
        }

        public static StringNode Op(BooleanNode bNode, StringNode sNode, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(bNode.Value + sNode.Value);

                default:
                    throw new Exception(BuildExMessage(bNode, sNode, op.ToString()));
            }
        }

        public static string BuildExMessage(TermNode leftType, TermNode rightType, string operation)
        {
            var builder = new StringBuilder();
            builder.Append("RUNTIME ERROR: \n Invalid operation performed! ");
            builder.Append("Types: " + leftType.ActualType.ToString() + " and " + rightType.ActualType.ToString() + " do not define operation: " + operation + "\n");
            builder.AppendLine("Error occured at line: " + leftType.Line);
            return builder.ToString();
        }
    }
}
