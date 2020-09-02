using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{   
    public sealed class Parser
    {
        readonly LanguageToken[] tokens;
        int pos;
        readonly Diagnostics diagnostics;
        
        public Parser(Diagnostics diag, LanguageToken[] allTokens)
        {
            diagnostics = diag;
            List<LanguageToken> scrubbedTokens = new List<LanguageToken>();
            foreach (LanguageToken t in allTokens)
            {
                if (t.tokenType != LanguageTokenType.BadToken && t.tokenType != LanguageTokenType.Whitespace)
                    scrubbedTokens.Add(t);
            }
            tokens = scrubbedTokens.ToArray();
            pos = 0;
        }

        public ExpressionNode ParseAll()
        { 
            ExpressionNode expr = ParseExpression();
            //matching for EOF token validates that we're indeed at the end of the file.
            LanguageToken eofToken = MatchToken(LanguageTokenType.EndOfFile);
            return expr;
        }

        private ExpressionNode ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private ExpressionNode ParseAssignmentExpression()
        {
            if (Peek().tokenType == LanguageTokenType.Identifier && Peek(1).tokenType == LanguageTokenType.Equals)
            {
                LanguageToken identifierToken = NextToken();
                LanguageToken operatorToken = NextToken();
                ExpressionNode expr = ParseAssignmentExpression();
                return new AssignmentExpression(identifierToken, expr, new TextSpan(identifierToken.textLocation.start, expr.textLocation.end));
            }
            return ParseBinaryExpression();
        }
        
        private ExpressionNode ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionNode left;
            int unaryOperatorPrecedence = Peek().GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                LanguageToken operatorToken = NextToken();
                ExpressionNode operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpression(operatorToken, operand, new TextSpan(operatorToken.textLocation.start, operand.textLocation.end));
            }
            else
            {
                left = ParsePrimaryExpression();
            }
            
            while (true)
            {
                int precedence = Peek().GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break; //parse later to maintain precedence order
                
                LanguageToken operatorToken = NextToken();
                ExpressionNode right = ParseBinaryExpression(precedence);
                left = new BinaryExpression(operatorToken, left, right, new TextSpan(left.textLocation.start, right.textLocation.end));
            }

            return left;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            LanguageToken current = Peek();
            TextSpan currentSpan = current.textLocation;

            switch (current.tokenType)
            {
                case LanguageTokenType.OpenParanthesis:
                {
                    LanguageToken left = NextToken();
                    ExpressionNode inner = ParseExpression();
                    LanguageToken right = MatchToken(LanguageTokenType.CloseParathesis);
                    return inner;
                }
                case LanguageTokenType.KeywordTrue:
                case LanguageTokenType.KeywordFalse:
                {
                    LanguageToken boolToken = NextToken();
                    bool value = boolToken.tokenType == LanguageTokenType.KeywordTrue;
                    return new LiteralExpression(value, boolToken, currentSpan);
                }
                case LanguageTokenType.Identifier:
                {
                    LanguageToken identifier = NextToken();
                    return new NameExpression(identifier, currentSpan);
                }

                default:
                {
                    //TODO: make this modular, not dependant on being int32. (move to switch)
                    LanguageToken token = MatchToken(LanguageTokenType.IntegerNumber);
                    if (!int.TryParse(token.text, out int val)) 
                        diagnostics.ParserError_CouldNotParseInt(currentSpan.start);

                    return new LiteralExpression(val, token, currentSpan);
                }
            }
        }

        private LanguageToken MatchToken(LanguageTokenType tokenType)
        {
            if (Peek().tokenType == tokenType)
                return NextToken();
            
            diagnostics.ParserError_TokenMatchFailed(tokenType, Peek().tokenType, Peek().textLocation.start);
            return new LanguageToken(tokenType, "", TextSpan.NoText);
        }

        private LanguageToken NextToken()
        {
            LanguageToken cur = Peek();
            pos++;
            return cur;
        }

        private LanguageToken Peek(int offset = 0)
        {
            if (pos + offset >= tokens.Length)
                return tokens[tokens.Length - 1]; //if we're OOB, just return the EOF
            return tokens[pos + offset];
        }
    }
}