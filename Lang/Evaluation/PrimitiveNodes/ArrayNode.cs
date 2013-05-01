using System.Linq;
<<<<<<< HEAD:Lang/Evaluation/PrimitiveNodes/ArrayNode.cs
<<<<<<< HEAD:Lang/Evaluation/PrimitiveNodes/ArrayNode.cs
using AEGIScript.Lang.Evaluation.ExpressionNodes;
=======
>>>>>>> 02d2e234ae3a1038fef2923d05ff58208dfe66a6:Lang/Evaluation/ArrayNode.cs
=======
<<<<<<< HEAD:Lang/Evaluation/ArrayNode.cs
=======
using AEGIScript.Lang.Evaluation.ExpressionNodes;
>>>>>>> Project restructured, geofactory support added:Lang/Evaluation/PrimitiveNodes/ArrayNode.cs
>>>>>>> detach:Lang/Evaluation/PrimitiveNodes/ArrayNode.cs
using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace AEGIScript.Lang.Evaluation.PrimitiveNodes
{
    class ArrayNode : TermNode
    {
        public ArrayNode(CommonTree tree, String content) : base(tree)
        {
        }

        public ArrayNode() : base()
        {
            ActualType = Type.Array;
        }

        public ArrayNode(List<TermNode> terms) : base()
        {
            Elements = terms;
            ActualType = Type.Array;
        }

        public List<TermNode> Elements = new List<TermNode>();

        public TermNode this[int index]
        {
            get
            {
                return Elements[index];
            }

            set
            {
                Elements[index] = value;
            }
        }

        public override TermNode CallFun(FunCallNode func)
        {
            Type[] actualTypes = func.ResolvedArgs.Select(x => x.ActualType).ToArray();

            switch (func.FunName)
            {
                case "Append":
                    return Call<TermNode>(new[] {Type.Any}, actualTypes, func, Append);
                case "Count":
                    return Call(func, Count);
                case "At":
                    return Call<IntNode>(new[] {Type.Int}, actualTypes, func, At);
                case "RemoveAt":
                    return Call<IntNode>(new[] { Type.Int }, actualTypes, func, RemoveAt);
                default:
                    return base.CallFun(func);
            }
        }

        private TermNode At(IntNode ind)
        {
            int index = ind.Value;
            if (index >= 0 && index < Elements.Count)
            {
                return Elements[index];
            }
            throw new Exception("RUNTIME ERROR!\n Index out of range!");
        }

        private BooleanNode Contains(TermNode term)
        {
            return new BooleanNode(Elements.Contains(term));
        }

        private TermNode Remove(TermNode term)
        {
            Elements.Remove(term);
            return this;
        }

        private TermNode RemoveAt(IntNode ind)
        {
            Elements.RemoveAt(ind.Value);
            return this;
        }

        private TermNode Append(TermNode term)
        {
            Elements.Add(term);
            return this;
        }

        private IntNode Count()
        {
            return new IntNode(Elements.Count);
        }

        public ArrayNode Add(TermNode node)
        {
            Elements.Add(node);
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[ ");
            if (Elements.Count > 0)
            {
                foreach (var elem in Elements)
                {
                    builder.Append(elem + " ");
                }
            }
            else if (Children != null && Children.Count > 0)
            {
                foreach (var elem in Children)
                {
                    builder.Append(elem + " ");
                }
            }

            builder.Append("]");
            return builder.ToString();
        }

        public override int GetHashCode()
        {
            return Elements.GetHashCode();
        }

    }
}
