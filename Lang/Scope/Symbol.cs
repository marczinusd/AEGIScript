using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Scope
{
    class Symbol
    {
        public Symbol(String name, Type t)
        {
            Name = name;
            T = t;
        }

        public String Name { get; private set; }
        protected Type T;
    }
}
