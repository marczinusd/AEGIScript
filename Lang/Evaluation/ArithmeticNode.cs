using Antlr.Runtime.Tree;
using System;

namespace AEGIScript.Lang.Evaluation
{
    class ArithmeticNode : ASTNode
    {
        public enum Operator { ADD, MULT, DIV, MOD, MIN, EQ, NEQ, GEQ, LEQ, GT, LT, OR, AND };

        public Operator op { get; private set; }

        public ArithmeticNode(CommonTree tree, String ops) : base(tree) 
        {
            switch (ops)
            {
                case "+":
                    op = Operator.ADD;
                    break;
                case "-":
                    op = Operator.MIN;
                    break;
                case "/":
                    op = Operator.DIV;
                    break;
                case "*":
                    op = Operator.MULT;
                    break;
                case "mod": case "%":
                    op = Operator.MOD;
                    break;
                case "!=":
                    op = Operator.NEQ;
                    break;
                case "==":
                    op = Operator.EQ;
                    break;
                case ">=":
                    op = Operator.GEQ;
                    break;
                case "<=":
                    op = Operator.LEQ;
                    break;
                case ">":
                    op = Operator.GT;
                    break;
                case "<":
                    op = Operator.LT;
                    break;
                case "||":
                    op = Operator.OR;
                    break;
                case "&&":
                    op = Operator.AND;
                    break;
                default:
                    break;
            }
            ActualType = Type.ARITH;
            dispose = false;
        }

        public override void Accept(IVisitor visitor)
        {
            base.Accept(visitor);
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
