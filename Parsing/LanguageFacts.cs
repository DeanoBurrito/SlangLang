using System;

namespace SlangLang.Parsing
{
    internal static class LanguageFacts
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
                    return 5;
                default:
                    return 0;
            }
        }


        public static ExpressionNodeType GetUnaryOperatorType(this LanguageToken token)
        {
            switch (token.tokenType)
            {
                case LanguageTokenType.Minus:
                    return ExpressionNodeType.Negate;
                case LanguageTokenType.Exclamation:
                    return ExpressionNodeType.Not;
                default:
                    return ExpressionNodeType.Nop;
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
                
                case LanguageTokenType.Plus:
                case LanguageTokenType.Minus:
                    return 3;
                case LanguageTokenType.Star:
                case LanguageTokenType.ForwardSlash:
                    return 4;
                default:
                    return 0;
            }
        }

        public static ExpressionNodeType GetBinaryOperatorType(this LanguageToken token)
        {
            switch (token.tokenType)
            {
                case LanguageTokenType.Plus:
                    return ExpressionNodeType.Addition;
                case LanguageTokenType.Minus:
                    return ExpressionNodeType.Subtraction;
                case LanguageTokenType.Star:
                    return ExpressionNodeType.Multiplication;
                case LanguageTokenType.ForwardSlash:
                    return ExpressionNodeType.Division;
                    
                case LanguageTokenType.PipePipe:
                    return ExpressionNodeType.ConditionalOr;
                case LanguageTokenType.AndAnd:
                    return ExpressionNodeType.ConditionalAnd;
                default:
                    return ExpressionNodeType.Nop;
            }
        }
    }
}