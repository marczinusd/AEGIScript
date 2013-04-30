using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AEGIScript.Lang.Evaluation;

namespace AEGIScript.Lang.Exceptions
{
    static class ExceptionGenerator
    {
        public static IndexOutOfRangeException IndexOutOfRange()
        {
            return new IndexOutOfRangeException();
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

    #region Exceptions
    [Serializable]
    public class InvalidCallException : Exception
    {
        public InvalidCallException()
        {
        }

        public InvalidCallException(string message)
            : base(message)
        {
        }

        public InvalidCallException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidCallException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidNodeOperationException : Exception
    {
        public InvalidNodeOperationException()
        {
        }

        public InvalidNodeOperationException(string message) : base(message)
        {
        }

        public InvalidNodeOperationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidNodeOperationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
#endregion
}
