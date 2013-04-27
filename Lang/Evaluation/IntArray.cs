using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;

namespace AEGIScript.Lang.Evaluation
{
    class IntArrayNode : ArrayNode
    {
        private List<int> Content = new List<int>();

        public IntArrayNode(CommonTree tree, String content) : base(tree, content)
        {
            ParseArrayElements();
        }

        private void ParseArrayElements()
        {
            foreach (ASTNode elem in Children)
            {
                try
                {
                    Content.Add(Int32.Parse(elem.Tree.Text));
                }
                catch (Exception ex)
                {
                    throw ex; // todo: implement invalid element exception
                }
            }
        }

        public int this[int key]
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
                else
                {
                    Content[key] = value;
                }
            }
        }
    }
}
