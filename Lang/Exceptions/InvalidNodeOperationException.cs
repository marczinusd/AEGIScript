using System;
using System.Runtime.Serialization;

namespace AEGIScript.Lang.Exceptions
{
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
}