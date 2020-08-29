using System;

namespace SlangLang.Parsing
{
    internal static class ParserFacts
    {
        public static int GetUnaryOperatorPrecedence(this LanguageToken token)
        {
            switch (token.tokenType)
            {
                case LanguageTokenType.Minus:
                    return 3;
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
                default:
                    return ExpressionNodeType.Nop;
            }
        }
        
        public static int GetBinaryOperatorPrecedence(this LanguageToken token)
        {
            //high precedence means operators are 'more sticky'. They will be applied and processed before lower precedence ones.
            switch (token.tokenType)
            {
                case LanguageTokenType.Plus:
                case LanguageTokenType.Minus:
                    return 1;
                case LanguageTokenType.Star:
                case LanguageTokenType.ForwardSlash:
                    return 2;
                default:
                    return 0;
            }
        }

        public static ExpressionNodeType GetBinaryOperatorType(this LanguageToken token)
        {
            switch (token.tokenType)
            {
                case LanguageTokenType.Plus:
                    return ExpressionNodeType.Add;
                case LanguageTokenType.Minus:
                    return ExpressionNodeType.Sub;
                case LanguageTokenType.Star:
                    return ExpressionNodeType.Mult;
                case LanguageTokenType.ForwardSlash:
                    return ExpressionNodeType.Div;
                default:
                    return ExpressionNodeType.Nop;
            }
        }
    }
}