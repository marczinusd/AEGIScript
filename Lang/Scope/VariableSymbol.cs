using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Scope
{
    class VariableSymbol : Symbol
    {
        public VariableSymbol(String name, Type type) : base(name, type)
        {
        }
    }
}
