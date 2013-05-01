using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using Antlr.Runtime.Tree;
using System;

namespace AEGIScript.Lang.Evaluation.ExpressionNodes
{
    class ArithmeticNode : ASTNode
    {
        public enum Operator { ADD, MULT, DIV, MOD, MIN, EQ, NEQ, GEQ, LEQ, GT, LT, OR, AND };

        public Operator Op { get; private set; }

        public ArithmeticNode(CommonTree tree, String ops) : base(tree) 
        {
            switch (ops)
            {
                case "+":
                    Op = Operator.ADD;
                    break;
                case "-":
                    Op = Operator.MIN;
                    break;
                case "/":
                    Op = Operator.DIV;
                    break;
                case "*":
                    Op = Operator.MULT;
                    break;
                case "mod": case "%":
                    Op = Operator.MOD;
                    break;
                case "!=":
                    Op = Operator.NEQ;
                    break;
                case "==":
                    Op = Operator.EQ;
                    break;
                case ">=":
                    Op = Operator.GEQ;
                    break;
                case "<=":
                    Op = Operator.LEQ;
                    break;
                case ">":
                    Op = Operator.GT;
                    break;
                case "<":
                    Op = Operator.LT;
                    break;
                case "||":
                    Op = Operator.OR;
                    break;
                case "&&":
                    Op = Operator.AND;
                    break;
            }
            ActualType = Type.Arith;
            Dispose = false;
        }

        #region Integer operations
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Minus(int a, int b)
        {
            return a - b;
        }

        public int Mult(int a, int b)
        {
            return a * b;
        }

        public int Div(int a, int b)
        {
            return a / b;
        }

        public int Mod(int a, int b)
        {
            return a % b;
        }
        #endregion

        #region Floating point operations
        public decimal Add(decimal a, decimal b)
        {
            return a + b;
        }

        public decimal Minus(decimal a, decimal b)
        {
            return a - b;
        }

        public decimal Mult(decimal a, decimal b)
        {
            return a * b;
        }

        public decimal Div(decimal a, decimal b)
        {
            return a / b;
        }

        public decimal Mod(decimal a, decimal b)
        {
            return a % b;
        }
        #endregion

        #region String operations
        public string Add(string a, string b)
        {
            return a + b;
        }
        #endregion



    }
}
