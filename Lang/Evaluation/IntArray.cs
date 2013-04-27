using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;

namespace AEGIScript.Lang.Evaluation
{
    class IntArrayNode : ArrayNode
    {
        private readonly List<int> Content = new List<int>();

        public IntArrayNode(CommonTree tree, String content) : base(tree, content)
        {
            ParseArrayElements();
        }

        private void ParseArrayElements()
        {
            foreach (ASTNode elem in Children)
            {
                Content.Add(Int32.Parse(elem.Tree.Text));
            }
        }

        public new int this[int key]
        {
            get
            {
                return Content[key];
            }
            set
            {
                if (Content.Count < key)
                {
                    throw new IndexOutOfRangeException();
                }
                Content[key] = value;
            }
        }
    }
}
