using System;
using System.Collections.Generic;
using SlangLang.Expressions;
using SlangLang.Debugging;

namespace SlangLang.Input
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
        
        //
        //
        //hacky
        public ExpressionNode ParseAll()
        { 
            ExpressionNode expr = ParseExpression();
            //validate EOF and ensure no extra data is sneaking into the pipeline (it'll be ignore, but still)
            //NOTE: Match() will throw an error if it dosnt find the eof token.
            LanguageToken eofToken = Match(LanguageTokenType.EndOfFile);
            return expr;
        }

        private ExpressionNode ParseExpression()
        {
            ExpressionNode left = ParsePrimaryExpression();
            while (Peek().tokenType == LanguageTokenType.Plus)
            {
                LanguageToken operatorToken = NextToken();
                ExpressionNode right = ParsePrimaryExpression();
                left = new BinaryExpression(ExpressionNodeType.Add, left, right);
            }

            return left;
        }

        private LanguageToken Match(LanguageTokenType tokenType)
        {
            if (Peek().tokenType == tokenType)
                return NextToken();
            
            diagnostics.AddFailure("Parser", "Token match failed, expecting " + tokenType + ", found " + Peek().tokenType + " instead.", TextLocation.NoLocation, DateTime.Now);
            return new LanguageToken(tokenType, "", TextLocation.NoLocation);
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            LanguageToken token = Match(LanguageTokenType.Integer);
            if (!int.TryParse(token.text, out int val))
                diagnostics.AddFailure("Parser", "Could not get int from number token.", token.sourceLocation, DateTime.Now);
            return new LiteralExpression(ExpressionNodeType.IntegerLiteral, val);
        }
        //hacks end here
        //
        //

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