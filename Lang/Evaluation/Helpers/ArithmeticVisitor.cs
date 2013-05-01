﻿using System;
<<<<<<< HEAD:Lang/Evaluation/Helpers/ArithmeticVisitor.cs
<<<<<<< HEAD:Lang/Evaluation/Helpers/ArithmeticVisitor.cs
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
=======
>>>>>>> 02d2e234ae3a1038fef2923d05ff58208dfe66a6:Lang/Evaluation/ArithmeticVisitor.cs
=======
<<<<<<< HEAD:Lang/Evaluation/ArithmeticVisitor.cs
=======
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
>>>>>>> Project restructured, geofactory support added:Lang/Evaluation/Helpers/ArithmeticVisitor.cs
>>>>>>> detach:Lang/Evaluation/Helpers/ArithmeticVisitor.cs

namespace AEGIScript.Lang.Evaluation.Helpers
{
    class ArithmeticVisitor : IVisitor
    {
        public void Visit(ASTNode node)
        {
            throw new NotImplementedException();
        }

        public int Visit(ArithmeticNode node)
        {
            switch (node.Op)
            {
                case ArithmeticNode.Operator.ADD:
                    //return Visit(node.Children[0].ActualNode) + Visit(node.Children[1].ActualNode));
                    break;
                case ArithmeticNode.Operator.MULT:
                    break;
                case ArithmeticNode.Operator.DIV:
                    break;
                case ArithmeticNode.Operator.MOD:
                    break;
                case ArithmeticNode.Operator.MIN:
                    break;
            }
            return 0;
        }

        public int Visit(IntNode left, IntNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return left.Value + right.Value;
                case ArithmeticNode.Operator.MULT:
                    return left.Value * right.Value;
                case ArithmeticNode.Operator.DIV:
                    return left.Value / right.Value;
                case ArithmeticNode.Operator.MOD:
                    return left.Value % right.Value;
                case ArithmeticNode.Operator.MIN:
                    return left.Value - right.Value;
                default:
                    throw new Exception("to implement"); // implement exception for this.
            }
        }

        public string Visit(StringNode str, IntNode i)
        {
            return str.Value + i;
        }

        public string Visit(StringNode str, DoubleNode d)
        {
            return str.Value + d;
        }

        public string Visit(StringNode left, StringNode right)
        {
            return left.Value + right.Value;
        }

        public double Visit(IntNode left, DoubleNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return left.Value + right.Value;
                case ArithmeticNode.Operator.MULT:
                    return left.Value * right.Value;
                case ArithmeticNode.Operator.DIV:
                    return left.Value / right.Value;
                case ArithmeticNode.Operator.MOD:
                    return left.Value % right.Value;
                case ArithmeticNode.Operator.MIN:
                    return left.Value - right.Value;
                default:
                    throw new Exception("to implement"); // implement exception for this.
            }
        }

        public double Visit(DoubleNode left, DoubleNode right, ArithmeticNode.Operator op)
        {
            switch (op)
            {
                case ArithmeticNode.Operator.ADD:
                    return left.Value + right.Value;
                case ArithmeticNode.Operator.MULT:
                    return left.Value * right.Value;
                case ArithmeticNode.Operator.DIV:
                    return left.Value / right.Value;
                case ArithmeticNode.Operator.MOD:
                    return left.Value % right.Value;
                case ArithmeticNode.Operator.MIN:
                    return left.Value - right.Value;
                default:
                    throw new Exception("to implement"); // implement exception for this.
            }
        }

        public int Visit(IntNode node)
        {
            return node.Value;
        }
    }
}
