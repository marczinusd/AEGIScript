using System;

namespace AEGIScript.IO
{
    public class ScriptIOException : Exception
    {
        public ScriptIOException(string path)
        {
            ThrownOnPath = path;
        }
        public override string Message
        {
            get
            {
                return base.Message + " \n File doesn't exist at : " + ThrownOnPath;
            }
        }
        public String ThrownOnPath { get; private set; }
    }
}