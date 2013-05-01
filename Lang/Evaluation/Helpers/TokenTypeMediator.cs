using System;

namespace AEGIScript.Lang.Evaluation.Helpers
{
    static class TokenTypeMediator
    {
        public static String[] TokenTypes { get; private set; }

        public static String GetTokenType(int tokenType)
        {
            if (tokenType >= 0 && tokenType < TokenTypes.Length)
            {
                return TokenTypes[tokenType];
            }
            return "";
        }

        public static void SetTokens(String[] tokens)
        {
            TokenTypes = tokens;
        }
    }
}
