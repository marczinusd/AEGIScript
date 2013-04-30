using System;

namespace AEGIScript.Lang.Exceptions
{
    class UndefinedVariableException : LangException
    {
        public UndefinedVariableException(String message) : base(message)
        {

        }

        public override string Message
        {
            get
            {
                return "Undefined variable: " + base.Message;
            }
        }
    }
}
