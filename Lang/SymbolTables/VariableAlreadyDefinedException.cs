using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGIScript.Lang.Scoping;

namespace AEGIScript.Lang.SymbolTables
{
    class VariableAlreadyDefinedException : Exception
    {

        public VariableAlreadyDefinedException(String message, String varName) : base(message) { VarName = varName; }

        public override string Message
        {
            get
            {
                return base.Message + "Error at variable: " + VarName ;
            }
        }

        private string VarName;
    }
}
