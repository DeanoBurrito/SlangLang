using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class Lexer
    {
        readonly TextStore sourceStore;
        int currChar;

        readonly string filename;
        readonly Diagnostics diagnostics;

        public Lexer(Diagnostics diag, string sourceLine, string sourceName)
        {
            sourceStore = new TextStore(new string[] {sourceLine} );
            diagnostics = diag;
            filename = sourceName;
            currChar = 0;
        }

        public LanguageToken[] LexAll()
        {
            List<LanguageToken> tokens = new List<LanguageToken>();
            LanguageToken token;
            do
            {
                token = LexNext();
                tokens.Add(token);
            }
            while (token.tokenType != LanguageTokenType.EndOfFile);
            return tokens.ToArray();
        }

        private LanguageToken LexNext()
        {
            if (currChar >= sourceStore.GetLength())
                return new LanguageToken(LanguageTokenType.EndOfFile, "", new TextLocation(filename, 0, 0));
            
            char current = sourceStore.GetCharAt(currChar);
            int start = currChar;
            TextLocation location = sourceStore.GetLocation(start);
            location.filename = filename;
            location.length = 1;
            if (char.IsWhiteSpace(current))
            {
                while (char.IsWhiteSpace(current))
                {
                    current = MoveNext();
                }
                location.length = currChar - start;
                return new LanguageToken(LanguageTokenType.Whitespace, sourceStore.GetSubstring(start, currChar - start), location);
            }
            if (char.IsDigit(current))
            {
                //some kind of number
                while (char.IsDigit(current))
                {
                    current = MoveNext();
                }
                location.length = currChar - start;
                return new LanguageToken(LanguageTokenType.IntegerNumber, sourceStore.GetSubstring(start, currChar - start), location);
            }

            if (current == '"')
            {
                //start of a string
                current = MoveNext();
                while (current != '"')
                {
                    current = MoveNext();
                    if (current == '\0')
                    {
                        diagnostics.AddFailure("Lexer", "Expected \" to end string literal, found end of file.", location, DateTime.Now);
                        return new LanguageToken(LanguageTokenType.EndOfFile, "", location);
                    }
                }
                MoveNext();
                location.length = currChar - start;
                return new LanguageToken(LanguageTokenType.String, sourceStore.GetSubstring(start, currChar - start), location);
            }

            switch (current)
            {
                case '+':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Plus, "+", location);
                case '-':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Minus, "-", location);
                case '*':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Star, "*", location);
                case '/':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.ForwardSlash, "/", location);
                case '!':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Exclamation, "!", location);
                case ';':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Semicolon, ";", location);
                case '=':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Equals, "=", location);
                case '(':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.OpenParanthesis, "(", location);
                case ')':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.CloseParathesis, "(", location);
            }

            diagnostics.AddFailure("Lexer", "Invalid character in input file '" + current + "'", location, DateTime.Now);
            currChar++;
            return new LanguageToken(LanguageTokenType.BadToken, "", location);
        }

        private char MoveNext()
        {
            currChar++;
            if (currChar >= sourceStore.GetLength())
                return '\0';
            return sourceStore.GetCharAt(currChar);
        }
    }
}