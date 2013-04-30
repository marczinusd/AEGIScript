using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Exceptions
{
    class LangException : Exception
    {
        public LangException(string message) : base(message) { }

        public LangException(string message, int atLine) : base(message) { Line = atLine; }

        public override String Message
        {
            get
            {
                return "Runtime error! " + base.Message;
            }
        }

        public int Line { get; private set; }
    }
}
