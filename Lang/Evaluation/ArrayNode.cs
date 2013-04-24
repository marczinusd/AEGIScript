using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ArrayNode Add(TermNode Node)
        {
            Elements.Add(Node);
            return this;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[ ");
            if (Elements.Count > 0)
            {
                foreach (var Elem in Elements)
                {
                    builder.Append(Elem.ToString() + " ");
                }
            }
            else if (Children.Count > 0)
            {
                foreach (var Elem in Children)
                {
                    builder.Append(Elem.ToString() + " ");
                }
            }

            builder.Append("]");
            return builder.ToString();
        }

    }
}
