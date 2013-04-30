using AEGIScript.Lang.Evaluation;
using System;

namespace AEGIScript.Lang.Exceptions
{
    class ASTEvalException : Exception
    {
        public ASTEvalException(String message, ASTNode node) : base(message)
        {
            RaisedFrom = node;
        }

        public ASTNode RaisedFrom { get; private set; }
    }
}
