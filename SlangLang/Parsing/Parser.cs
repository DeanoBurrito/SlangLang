using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SlangLang.Debug;

namespace SlangLang.Parsing
{   
    public sealed class Parser
    {
        readonly ImmutableArray<LanguageToken> tokens;
        readonly Diagnostics diagnostics;
        int pos;
        
        public Parser(Diagnostics diag, LanguageToken[] allTokens)
        {
            diagnostics = diag;
            List<LanguageToken> scrubbedTokens = new List<LanguageToken>();
            foreach (LanguageToken t in allTokens)
            {
                if (t.tokenType != LanguageTokenType.BadToken && t.tokenType != LanguageTokenType.Whitespace)
                    scrubbedTokens.Add(t);
            }
            tokens = scrubbedTokens.ToImmutableArray();
            pos = 0;
        }

        public Parser(Diagnostics diag, TextStore sourceText)
        {
            diagnostics = diag;
            Lexer lexer = new Lexer(diag, sourceText);
            List<LanguageToken> rawTokens = new List<LanguageToken>(lexer.LexAll());
            ImmutableArray<LanguageToken>.Builder tokenBuilder = ImmutableArray.CreateBuilder<LanguageToken>();
            foreach (LanguageToken t in rawTokens)
            {
                if (t.tokenType != LanguageTokenType.BadToken && t.tokenType != LanguageTokenType.Whitespace)
                    tokenBuilder.Add(t);
            }
            tokens = tokenBuilder.ToImmutableArray();
            pos = 0;
        }

        public CompilationUnit ParseCompilationUnit()
        { 
            StatementNode statement = ParseStatement();
            
            //matching for EOF token validates that we're indeed at the end of the file.
            LanguageToken eofToken = MatchToken(LanguageTokenType.EndOfFile);
            return new CompilationUnit(statement, eofToken, TextSpan.NoText);
        }

        private StatementNode ParseStatement()
        {
            LanguageToken current = Peek(); //TODO: move this into a switch statement
            if (current.tokenType == LanguageTokenType.OpenBrace)
                return ParseBlockStatement();
            else if (LanguageFacts.KeywordIsVariableType(current.tokenType) || current.tokenType == LanguageTokenType.KeywordLet)
                return ParseVariableDeclaration();
            else if (current.tokenType == LanguageTokenType.KeywordIf)
                return ParseIfStatement();
            else if (current.tokenType == LanguageTokenType.KeywordWhile)
                return ParseWhileStatement();
            else if (current.tokenType == LanguageTokenType.KeywordFor)
                return ParseForStatement();
            return ParseExpressionStatement();
        }

        private BlockStatement ParseBlockStatement()
        {
            ImmutableArray<StatementNode>.Builder statements = ImmutableArray.CreateBuilder<StatementNode>();
            LanguageToken openBrace = MatchToken(LanguageTokenType.OpenBrace);

            while (Peek().tokenType != LanguageTokenType.EndOfFile && Peek().tokenType != LanguageTokenType.CloseBrace)
            {
                int startPosition = pos;
                StatementNode statement = ParseStatement();
                statements.Add(statement);

                if (pos == startPosition)
                {
                    //we havent moved, just produced a synthesized token to prevent tree from crashing, time to escape infinite loop
                    //error will already have been reported from MatchToken() at some point.
                    NextToken();
                }
                startPosition = pos;
            }

            LanguageToken closeBrace = MatchToken(LanguageTokenType.CloseBrace);
            return new BlockStatement(openBrace, statements.ToImmutable(), closeBrace);
        }

        private VariableDeclarationStatement ParseVariableDeclaration()
        {
            bool varIsReadonly = Peek().tokenType == LanguageTokenType.KeywordLet;
            if (varIsReadonly)
                NextToken();

            LanguageToken keyword = MatchToken(LanguageTokenType.KeywordInt); //TODO: accept more variable types
            LanguageToken identifier = MatchToken(LanguageTokenType.Identifier);
            LanguageToken equals = MatchToken(LanguageTokenType.Equals);
            ExpressionNode initializer = ParseExpression();
            LanguageToken semicolon = MatchToken(LanguageTokenType.Semicolon);
            return new VariableDeclarationStatement(keyword, identifier, equals, initializer, semicolon, varIsReadonly, 
                new TextSpan(keyword.textLocation.start, equals.textLocation.end));
        }

        private IfStatement ParseIfStatement()
        {
            LanguageToken keyword = MatchToken(LanguageTokenType.KeywordIf);
            ExpressionNode condition = ParseExpression();
            StatementNode body = ParseStatement();
            ElseClauseData elseClause = ParseElseClause();
            return new IfStatement(keyword, condition, body, elseClause, new TextSpan(keyword.textLocation.start, condition.textLocation.end));
        }

        private WhileStatement ParseWhileStatement()
        {
            LanguageToken keyword = MatchToken(LanguageTokenType.KeywordWhile);
            ExpressionNode condition = ParseExpression();
            StatementNode body = ParseStatement();
            return new WhileStatement(keyword, condition, body, new TextSpan(keyword.textLocation.start, condition.textLocation.end));
        }

        private ForStatement ParseForStatement()
        {
            LanguageToken keyword = MatchToken(LanguageTokenType.KeywordFor);
            StatementNode setup = ParseStatement();
            ExpressionNode condition = ParseExpression();
            MatchToken(LanguageTokenType.Semicolon); //since condition is an expression, not a statement. Expect a semicolon for consistency
            StatementNode post = ParseStatement();
            StatementNode body = ParseStatement();
            return new ForStatement(keyword, setup, condition, post, body, new TextSpan(keyword.textLocation.start, condition.textLocation.end));
        }

        private ElseClauseData ParseElseClause()
        {
            if (Peek().tokenType != LanguageTokenType.KeywordElse)
                return null;
            LanguageToken elseKeyword = MatchToken(LanguageTokenType.KeywordElse);
            StatementNode elseBody = ParseStatement();
            return new ElseClauseData(elseKeyword, elseBody);
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            ExpressionNode expression = ParseExpression();
            LanguageToken semicolon = MatchToken(LanguageTokenType.Semicolon);
            return new ExpressionStatement(expression, semicolon);
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
                return new AssignmentExpression(identifierToken, expr, expr.textLocation);
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
                left = new UnaryExpression(operatorToken, operand, operand.textLocation);
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
                left = new BinaryExpression(operatorToken, left, right, operatorToken.textLocation);
            }

            return left;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            LanguageToken current = Peek();
            switch (current.tokenType)
            {
                case LanguageTokenType.OpenParanthesis:
                    return ParseParanthesizedExpression();
                case LanguageTokenType.KeywordTrue:
                case LanguageTokenType.KeywordFalse:
                    return ParseBoolLiteral();
                case LanguageTokenType.IntegerNumber:
                    return ParseIntegerLiteral();
                case LanguageTokenType.Identifier:
                default: 
                    return ParseNameExpression();
            }
        }

        private ExpressionNode ParseNameExpression()
        {
            LanguageToken identifier = MatchToken(LanguageTokenType.Identifier);
            return new NameExpression(identifier, identifier.textLocation);
        }

        private ExpressionNode ParseParanthesizedExpression()
        {
            LanguageToken left = MatchToken(LanguageTokenType.OpenParanthesis);
            ExpressionNode inner = ParseExpression();
            LanguageToken right = MatchToken(LanguageTokenType.CloseParathesis);
            return inner;
        }
        
        private ExpressionNode ParseBoolLiteral()
        {
            bool isTrue = Peek().tokenType == LanguageTokenType.KeywordTrue;
            LanguageToken boolToken = MatchToken(isTrue ? LanguageTokenType.KeywordTrue : LanguageTokenType.KeywordFalse);
            return new LiteralExpression(isTrue, boolToken, boolToken.textLocation);
        }

        private ExpressionNode ParseIntegerLiteral()
        {
            LanguageToken current = Peek();
            LanguageToken token = MatchToken(LanguageTokenType.IntegerNumber);

            if (!int.TryParse(token.text, out int val)) 
                diagnostics.ParserError_CouldNotParseInt(current.textLocation.start);

            return new LiteralExpression(val, token, current.textLocation);
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