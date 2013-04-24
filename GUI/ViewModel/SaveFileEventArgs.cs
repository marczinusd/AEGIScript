using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.GUI.ViewModel
{
    class SaveFileEventArgs : EventArgs
    {
        public SaveFileEventArgs(TextDocument _doc, string _path)
        {
            Doc = _doc;
            Path = _path;
        }

        public TextDocument Doc { get; private set; }
        public string Path { get; private set; }
    }
}
