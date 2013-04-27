using AEGIScript.Lang.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AEGIScript.Lang.SymbolTables
{


    class SymbolTable
    {
        private Dictionary<String, TermNode> fields = new Dictionary<string, TermNode>();

        public void Clear()
        {
            fields.Clear();
        }

        public void AddVar(String symbol, TermNode node)
        {
            if(fields.Keys.Contains(symbol))
            {
                fields[symbol] = node;
            }
            else
            {
                fields.Add(symbol, node);
            }
        }

        public TermNode GetVar(String symbol)
        {
            if (fields.Keys.Contains(symbol))
            {
                return fields[symbol];
            }
            throw new Exception("RUNTIME ERROR! \n Undefined variable: " + symbol + "\n");
        }

        public bool HasVar(String symbol)
        {
            return fields.ContainsKey(symbol);
        }

    }
}
