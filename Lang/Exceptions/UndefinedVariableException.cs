using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGIScript.Lang.Exceptions;

namespace AEGIScript.Lang.Exceptions
{
    class UndefinedVariableException : LangException
    {
        public UndefinedVariableException(String Message) : base(Message)
        {

        }

        public override string Message
        {
            get
            {
                return "Undefined variable: " + Message;
            }
        }
    }
}
