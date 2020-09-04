using System;
using System.Collections.Generic;

namespace SlangLang.Parsing
{
    public static class LanguageFacts
    {
        public static LanguageTokenType GetKeyword(string text)
        {
            switch (text)
            {
                case "true":
                    return LanguageTokenType.KeywordTrue;
                case "false":
                    return LanguageTokenType.KeywordFalse;
                default:
                    return LanguageTokenType.Identifier;
            }
        }
        
        public static int GetUnaryOperatorPrecedence(this LanguageToken token)
        {
            switch (token.tokenType)
            {
                case LanguageTokenType.Minus:
                case LanguageTokenType.Exclamation:
                    return 6;
                default:
                    return 0;
            }
        }
        
        public static int GetBinaryOperatorPrecedence(this LanguageToken token)
        {
            //high precedence means operators are 'more sticky'. They will be applied and processed before lower precedence ones.
            switch (token.tokenType)
            {
                case LanguageTokenType.Pipe:
                case LanguageTokenType.PipePipe:
                    return 1;
                case LanguageTokenType.And:
                case LanguageTokenType.AndAnd:
                    return 2;
                
                case LanguageTokenType.EqualsEquals:
                case LanguageTokenType.ExclamationEquals:
                    return 3;
                
                case LanguageTokenType.Plus:
                case LanguageTokenType.Minus:
                    return 4;
                case LanguageTokenType.Star:
                case LanguageTokenType.ForwardSlash:
                    return 5;
                default:
                    return 0;
            }
        }

        public static IEnumerable<LanguageTokenType> GetUnaryOperators()
        {
            LanguageTokenType[] allTypes = (LanguageTokenType[])Enum.GetValues(typeof(LanguageTokenType));
            foreach (LanguageTokenType type in allTypes)
            {
                if (GetUnaryOperatorPrecedence(new LanguageToken(type, "", null)) > 0)
                    yield return type;
            }
        }

        public static IEnumerable<LanguageTokenType> GetBinaryOperators()
        {
            LanguageTokenType[] allTypes = (LanguageTokenType[])Enum.GetValues(typeof(LanguageTokenType));
            foreach (LanguageTokenType type in allTypes)
            {
                if (GetBinaryOperatorPrecedence(new LanguageToken(type, "", null)) > 0)
                    yield return type;
            }
        }

        public static string GetText(LanguageTokenType token)
        {
            switch (token)
            {
                case LanguageTokenType.And:
                    return "&";
                case LanguageTokenType.AndAnd:
                    return "&&";
                case LanguageTokenType.Pipe:
                    return "|";
                case LanguageTokenType.PipePipe:
                    return "||";
                case LanguageTokenType.Equals:
                    return "=";
                case LanguageTokenType.EqualsEquals:
                    return "==";
                case LanguageTokenType.Exclamation:
                    return "!";
                case LanguageTokenType.ExclamationEquals:
                    return "!=";

                case LanguageTokenType.KeywordFalse:
                    return "false";
                case LanguageTokenType.KeywordTrue:
                    return "true";
                
                case LanguageTokenType.Plus:
                    return "+";
                case LanguageTokenType.Minus:
                    return "-";
                case LanguageTokenType.Star:
                    return "*";
                case LanguageTokenType.ForwardSlash:
                    return "/";

                case LanguageTokenType.OpenParanthesis:
                    return "(";
                case LanguageTokenType.CloseParathesis:
                    return ")";
                case LanguageTokenType.OpenBrace:
                    return "{";
                case LanguageTokenType.CloseBrace:
                    return "}";
                case LanguageTokenType.Semicolon:
                    return ";";
                default:
                    return null;
            }
        }
    }
}