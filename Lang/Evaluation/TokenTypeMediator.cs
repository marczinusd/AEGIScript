using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Evaluation
{
    static class TokenTypeMediator
    {
        public static String[] TokenTypes { get; private set; }

        public static String GetTokenType(int TokenType)
        {
            if (TokenType >= 0 && TokenType < TokenTypes.Length)
            {
                return TokenTypes[TokenType];
            }
            return "";
        }

        public static void SetTokens(String[] tokens)
        {
            TokenTypes = tokens;
        }
    }
}
