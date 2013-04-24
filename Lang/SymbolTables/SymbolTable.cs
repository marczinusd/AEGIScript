using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGIScript.Lang.Scope;
using AEGIScript.Lang.Evaluation;
using AEGIScript.Lang.Exceptions;

namespace AEGIScript.Lang.SymbolTables
{


    class SymbolTable
    {
        // TODO : realloc, check for symbol in all dictionaries
        public SymbolTable pred;

        private Dictionary<String, TermNode> fields = new Dictionary<string, TermNode>();
        private Dictionary<String, String> strings = new Dictionary<string, string>();
        private Dictionary<String, Int32> ints = new Dictionary<string, int>();
        private Dictionary<String, Double> doubles = new Dictionary<string, double>();
        private Dictionary<String, Boolean> bools = new Dictionary<string, Boolean>();

        public Boolean IsAssigned(String symbol)
        {
            return strings.Keys.Contains(symbol) || ints.Keys.Contains(symbol) || doubles.Keys.Contains(symbol) || bools.Keys.Contains(symbol);
        }

        public void Clear()
        {
            fields.Clear();
            strings.Clear();
            ints.Clear();
            doubles.Clear();
            bools.Clear();
        }

        public void AddVar(String symbol, TermNode node)
        {
            switch (node.ActualType)
            {
                case ASTNode.Type.INT:
                    AddVarToScope(symbol, (node as IntNode).Value);
                    break;
                case ASTNode.Type.STRING:
                    AddVarToScope(symbol, (node as StringNode).Value);
                    break;
                case ASTNode.Type.BOOL:
                    AddVarToScope(symbol, (node as BooleanNode).Value);
                    break;
                case ASTNode.Type.DOUBLE:
                    AddVarToScope(symbol, (node as DoubleNode).Value);
                    break;
            }
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
            else
            {
                throw new Exception("RUNTIME ERROR! \n Undefined variable: " + symbol + "\n");
            }
        }

        public bool HasVar(String symbol)
        {
            return fields.ContainsKey(symbol);
        }

        public Double GetDouble(String symbol)
        {
            try
            {
                return doubles[symbol];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Int32 GetInt(String symbol)
        {
            try
            {
                return ints[symbol];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetString(String symbol)
        {
            try
            {
                return strings[symbol];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean GetBool(String symbol)
        {
            try
            {
                return bools[symbol];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddVarToScope(String name, Int32 var)
        {
            if (ints.Keys.Contains(name))
            {
                ints[name] = var;
            }
            else
            {
                ints.Add(name, var);
            }
            
        }

        public void AddVarToScope(String name, Double var)
        {
            if (doubles.Keys.Contains(name))
            {
                doubles[name] = var;
            }
            else
            {
                doubles.Add(name, var);
            }
        }

        public void AddVarToScope(String name, String var)
        {
            if (strings.Keys.Contains(name))
            {
                strings[name] = var;
            }
            else
            {
                strings.Add(name, var);
            }
        }

        public void AddVarToScope(String name, Boolean var)
        {
            if (bools.Keys.Contains(name))
            {
                bools[name] = var;
            }
            else
            {
                bools.Add(name, var);
            }
        }

    }
}
