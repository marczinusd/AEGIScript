using System;
using System.Runtime.Serialization;

namespace AEGIScript.Lang.Exceptions
{
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
}