using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEGIScript.Lang.Evaluation;

namespace AEGIScript.GUI.Model
{
    class PrintEventArgs : EventArgs
    {
        public PrintEventArgs(TermNode node) : base()
        {
            Node = node;
        }

        public PrintEventArgs(String ToPrint) : base()
        {
            Message = ToPrint;
        }

        public TermNode Node { get; private set; }
        public String Message { get; private set; }

        public override string ToString()
        {
            if (Node != null)
            {
                return Node.ToString();   
            }
            else
            {
                return Message;
            }
        }
    }
}
