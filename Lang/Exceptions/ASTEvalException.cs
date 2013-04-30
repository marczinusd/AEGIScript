using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGIScript.Lang.Evaluation;

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
