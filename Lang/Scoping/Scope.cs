using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGIScript.Lang.SymbolTables;
using AEGIScript.Lang.Evaluation;


namespace AEGIScript.Lang.Scoping
{
    class Scope
    {
        public Scope(Scope precedingScope = null)
        {
            PrecedingScope = precedingScope;
            GlobalScope = new SymbolTable();
            Tables = new List<SymbolTable>();
        }

        public TermNode GetVar(String Symbol)
        {
            for (int i = Tables.Count - 1; i >= 0; i--)
            {
                if (Tables[i].HasVar(Symbol))
                {
                    return Tables[i].GetVar(Symbol);
                }
            }
            if (GlobalScope.HasVar(Symbol))
            {
                return GlobalScope.GetVar(Symbol);
            }
            else
	        {
                throw new Exception("RUNTIME ERROR! \n Undefined variable: " + Symbol + "\n");
	        }

        }

        public void AddVar(String Symbol, TermNode Node)
        {
            if (Tables.Count > 0)
            {
                for (int i = Tables.Count - 1; i >= 0; i--)
                {
                    if (Tables[i].HasVar(Symbol))
                    {
                        Tables[i].AddVar(Symbol, Node);
                        return;
                    }
                }
                if (GlobalScope.HasVar(Symbol))
                {
                    GlobalScope.AddVar(Symbol, Node);
                    return;
                }
                else Tables[Tables.Count - 1].AddVar(Symbol, Node);
            }
            else
            {
                GlobalScope.AddVar(Symbol, Node);
            }
        }

        public void NewScope()
        {
            Tables.Add(new SymbolTable());
        }

        public void RemoveScope()
        {
            if (Tables.Count > 0)
            {
                Tables.RemoveAt(Tables.Count - 1);
            }
        }

        public void Clear()
        {
            Tables.Clear();
            GlobalScope.Clear();
        }

        private SymbolTable GlobalScope { get; set; }
        private List<SymbolTable> Tables { get; set; }

        public SymbolTable fields { get; private set; }
        protected Scope PrecedingScope;
    }
}
