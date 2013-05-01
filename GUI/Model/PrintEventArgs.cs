using System;
using AEGIScript.Lang.Evaluation;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;

namespace AEGIScript.GUI.Model
{
    class PrintEventArgs : EventArgs
    {
        public PrintEventArgs(TermNode node)
        {
            Node = node;
        }

        public PrintEventArgs(String toPrint)
        {
            Message = toPrint;
        }
        public TermNode Node { get; private set; }
        public String Message { get; private set; }

        public override string ToString()
        {
            return Node != null ? Node.ToString() : Message;
        }
    }

}
