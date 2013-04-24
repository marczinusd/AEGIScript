using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Exceptions
{
    class InvalidOpsException : LangException
    {
        public InvalidOpsException(string Message) : base(Message) { }

        public override string Message
        {
            get
            {
                return "Invalid operation! Reason: " + Message;
            }
        }
    }
}
