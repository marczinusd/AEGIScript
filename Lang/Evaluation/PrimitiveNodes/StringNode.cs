using System;
using System.Linq;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Exceptions;
using AEGIScript.Lang.Scoping;
using Antlr.Runtime.Tree;

namespace AEGIScript.Lang.Evaluation.PrimitiveNodes
{
    class StringNode : TermNode
    {
        public StringNode(CommonTree tree, String content) : base(tree)
        {
            Value = content.Trim(new[]{'"'});
            ActualType = Type.String;
        }

        public StringNode(String value)
        {
            Value = value;
            ActualType = Type.String;
        }

        public String Interpret(SymbolTable symbols)
        {
            return "";
        }

        public override TermNode CallFun(FunCallNode func)
        {
            Type[] actualTypes = func.ResolvedArgs.Select(x => x.ActualType).ToArray();
            switch (func.FunName)
            {
                case "Count":
                    return Call(func, Count);
                case "Contains":
                    return Call<StringNode>(new[] {Type.String}, actualTypes, func, Contains);
                case "Substring":
                    if (actualTypes.Length == 1)
                        return Call<IntNode>(new[] { Type.Int }, actualTypes, func, Substring);
                    else if (actualTypes.Length == 2)
                        return Call<IntNode, IntNode>(new[] {Type.Int, Type.Int}, actualTypes, func, Substring);
                    else throw ExceptionGenerator.BadArity(func);
                case "Trim":
                    return Call(func, Trim);
                default:
                    return base.CallFun(func);
            }
        }


        private IntNode Count()
        {
            return new IntNode(Value.Length);
        }

        public BooleanNode Contains(StringNode str)
        {
            return new BooleanNode(Value.Contains(str.Value));
        }

        public StringNode Substring(IntNode from)
        {
            return new StringNode(Value.Substring(from.Value));
        }

        public StringNode Substring(IntNode begin, IntNode end)
        {
            return new StringNode(Value.Substring(begin.Value, end.Value));
        }

        public StringNode Trim()
        {
            return new StringNode(Value.Trim());
        }

        public string Value { get; private set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
