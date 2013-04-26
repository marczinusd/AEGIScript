﻿using Antlr.Runtime.Tree;
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

        public ArrayNode() : base()
        {
            this.ActualType = Type.ARRAY;
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
            switch (func.FunName)
            {
                case "Append":
                    if (func.ResolvedArgs.Count == 1)
                    {
                        return Append(func.ResolvedArgs[0]);
                    }
                    else throw new Exception(func.BadCallMessage());
                case "Count":
                    if (func.ResolvedArgs.Count == 0)
                    {
                        return Count();
                    }
                    else throw new Exception(func.BadCallMessage());
                default:
                    throw new Exception(func.BadCallMessage());
            }
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

    }
}
