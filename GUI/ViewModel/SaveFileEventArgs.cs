using ICSharpCode.AvalonEdit.Document;
using System;

namespace AEGIScript.GUI.ViewModel
{
    class SaveFileEventArgs : EventArgs
    {
        public SaveFileEventArgs(TextDocument doc, string path)
        {
            Doc = doc;
            Path = path;
        }

        public TextDocument Doc { get; private set; }
        public string Path { get; private set; }
    }
}
