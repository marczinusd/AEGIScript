namespace AEGIScript.Lang.Exceptions
{
    class InvalidOpsException : LangException
    {
        public InvalidOpsException(string message) : base(message) { }

        public override string Message
        {
            get
            {
                return "Invalid operation! Reason: " + base.Message;
            }
        }
    }
}
