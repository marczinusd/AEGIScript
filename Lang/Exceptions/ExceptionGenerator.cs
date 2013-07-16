using System;
using System.IO;
using System.Linq;
using System.Text;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;

namespace AEGIScript.Lang.Exceptions
{
    /// <summary>
    ///     Static class to generate verbose error messages for semantic and runtime errors.
    /// </summary>
    static class ExceptionGenerator
    {
        public static IndexOutOfRangeException IndexOutOfRange(ArrAccessNode arr)
        {
            return new IndexOutOfRangeException("RUNTIME ERROR!\n You are trying to access an array with an out of range index! Error occured at line: " + arr.Line);
        }

        public static EndOfStreamException EndOfStream()
        {
            return new EndOfStreamException("RUNTIME ERROR!\n You are trying to read from an empty stream!");
        }

        public static InvalidCallException UndefinedFunctionCall(FunCallNode func)
        {
            return new InvalidCallException();
        }

        public static InvalidCallException BadArity(FunCallNode func)
        {
            return new InvalidCallException("RUNTIME ERROR!\n Invalid call to: " + func.FunName + " on line : " 
                + func.Line + ".\n The function has no overload that takes " + func.ResolvedArgs.Count + " parameters.");
        }

        public static InvalidCallException BadArguments(FunCallNode func)
        {
            var errorMsg = new StringBuilder();
            errorMsg.AppendLine("RUNTIME ERROR!\n Invalid arguments in function call to "
                + func.FunName + " on line " + func.Line);
            errorMsg.Append("Called with types:");
            foreach (var type in func.ResolvedArgs.Select(x => x.ActualType))
            {
                errorMsg.Append(type.ToString() + " ");
            }
            return new InvalidCallException(errorMsg.ToString());
        }

        public static InvalidCallException BadArguments(FunCallNode func, ASTNode.Type[] actualSignature)
        {
            var errorMsg = new StringBuilder();
            errorMsg.AppendLine("RUNTIME ERROR!\n Invalid arguments in function call to " 
                + func.FunName + " on line " + func.Line);
            errorMsg.Append("Called with types:");
            foreach (var type in func.ResolvedArgs.Select(x => x.ActualType))
            {
                errorMsg.Append(type.ToString() + " ");
            }
            errorMsg.Append(", but function takes parameters ");
            foreach (var type in actualSignature)
            {
                errorMsg.Append(" " + type.ToString());
            }
            return new InvalidCallException(errorMsg.ToString());
        }

        public static InvalidCallException UndefinedFunction(FunCallNode func, ASTNode.Type onType)
        {
            return new InvalidCallException("RUNTIME ERROR!\n Undefined function call to " + func.FunName 
                + " on type: " + onType.ToString() + " on line: " + func.Line);
        }

        public static InvalidNodeOperationException UndefinedOperation(ASTNode left, ASTNode right, ArithmeticNode.Operator op)
        {
            var builder = new StringBuilder();
            builder.Append("RUNTIME ERROR: \n Invalid operation performed! ");
            builder.Append("Types: " + left.ActualType.ToString() + " and " + right.ActualType.ToString() + " do not define operation: " + op + "\n");
            builder.AppendLine("Error occured at line: " + left.Line);
            return new InvalidNodeOperationException(builder.ToString());
        }

    }
}
