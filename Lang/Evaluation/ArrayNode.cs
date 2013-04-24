using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace AEGIScript.Lang.Evaluation
{
    class ArrayNode : TermNode
    {
        public ArrayNode(CommonTree tree, String content) : base(tree, content)
        {
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
            else if (Children.Count > 0)
            {
                foreach (var elem in Children)
                {
                    builder.Append(elem + " ");
                }
            }

            builder.Append("]");
            return builder.ToString();
        }

    }
}
