using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;

namespace AEGIScript.Lang.Evaluation
{
    class IntArrayNode : ArrayNode
    {
        private readonly List<int> _content = new List<int>();

        public IntArrayNode(CommonTree tree, String content) : base(tree, content)
        {
            ParseArrayElements();
        }

        private void ParseArrayElements()
        {
            foreach (ASTNode elem in Children)
            {
                _content.Add(Int32.Parse(elem.Tree.Text));
            }
        }

        public new int this[int key]
        {
            get
            {
                return _content[key];
            }
            set
            {
                if (_content.Count < key)
                {
                    throw new IndexOutOfRangeException();
                }
                _content[key] = value;
            }
        }
    }
}
