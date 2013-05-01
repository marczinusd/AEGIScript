using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using AEGIScript.Lang.Exceptions;

namespace AEGIScript.Lang.Evaluation.Helpers
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
                            throw ExceptionGenerator.UndefinedOperation(left, right, op);
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
                            throw ExceptionGenerator.UndefinedOperation(left, right, op);
                    }
                case ASTNode.Type.Bool:
                    switch (right.ActualType)
                    {
                        case ASTNode.Type.Bool:
                            return Op(left as BooleanNode, right as BooleanNode, op);
                        default:
                            throw ExceptionGenerator.UndefinedOperation(left, right, op);
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
                            throw ExceptionGenerator.UndefinedOperation(left, right, op);
                    }
                case ASTNode.Type.Array:
                    return Op(left as ArrayNode, right, op);
                case ASTNode.Type.Negation:
                    if (right.ActualType == ASTNode.Type.Bool)
                    {
                        return new BooleanNode(!(((BooleanNode) right).Value));
                    }
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
                case ASTNode.Type.Negative:
                    switch (right.ActualType)
                    {
                        case ASTNode.Type.Int:
                            return new IntNode(-(((IntNode) right).Value));
                        case ASTNode.Type.Double:
                            return new DoubleNode(-(((DoubleNode) right).Value));
                        default:
                            throw ExceptionGenerator.UndefinedOperation(left, right, op);
                    }
                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }
        #endregion
        #endregion

        public static TermNode Op(DoubleNode left, IntNode right, ArithmeticNode.Operator op)
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
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
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
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
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
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static TermNode Op(NegationNode left, BooleanNode right, ArithmeticNode.Operator op)
        {
            return new BooleanNode(!right.Value);
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
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
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
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static TermNode Op(ArrayNode left, TermNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return left.Add(right);
                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
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
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static StringNode Op(StringNode left, IntNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(left.Value + right.Value);
                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static StringNode Op(IntNode left, StringNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(left.Value + right.Value);
                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static StringNode Op(StringNode left, DoubleNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(left.Value + right.Value);
                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static StringNode Op(DoubleNode left, StringNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(left.Value + right.Value);
                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static StringNode Op(StringNode left, BooleanNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(left.Value + right.Value);

                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        public static StringNode Op(BooleanNode left, StringNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return new StringNode(left.Value + right.Value);

                default:
                    throw ExceptionGenerator.UndefinedOperation(left, right, op);
            }
        }

        //public static string BuildExMessage(TermNode leftType, TermNode rightType, string operation)
        //{
        //    var builder = new StringBuilder();
        //    builder.Append("RUNTIME ERROR: \n Invalid operation performed! ");
        //    builder.Append("Types: " + leftType.ActualType.ToString() + " and " + rightType.ActualType.ToString() + " do not define operation: " + operation + "\n");
        //    builder.AppendLine("Error occured at line: " + leftType.Line);
        //    return builder.ToString();
        //}
    }
}
