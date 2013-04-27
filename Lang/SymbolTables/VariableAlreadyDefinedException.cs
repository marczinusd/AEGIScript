using System;

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

        private readonly string VarName;
    }
}
