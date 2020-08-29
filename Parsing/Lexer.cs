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
                //whitespace
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
                //start of a string literal
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

            if (char.IsLetter(current))
            {
                //text
                while (char.IsLetter(current))
                {
                    current = MoveNext();
                }

                location.length = currChar - start;
                string text = sourceStore.GetSubstring(start, currChar - start);
                LanguageTokenType keyword = LanguageFacts.GetKeyword(text);
                return new LanguageToken(keyword, text, location);
            }

            switch (current)
            {
                case '+':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.Plus, "+", location);
                case '-':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.Minus, "-", location);
                case '*':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.Star, "*", location);
                case '/':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.ForwardSlash, "/", location);
                case '!':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.Exclamation, "!", location);
                case ';':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.Semicolon, ";", location);
                case '(':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.OpenParanthesis, "(", location);
                case ')':
                    MoveNext();
                    return new LanguageToken(LanguageTokenType.CloseParathesis, "(", location);
                case '=':
                    if (PeekNext(1) == '=')
                    {
                        MoveNext(); MoveNext();
                        location.length = 2;
                        return new LanguageToken(LanguageTokenType.EqualsEquals, "==", location);
                    }
                    else
                    {
                        MoveNext();
                        return new LanguageToken(LanguageTokenType.Equals, "=", location);
                    }
                case '&':
                    if (PeekNext(1) == '&')
                    {
                        MoveNext(); MoveNext();
                        location.length = 2;
                        return new LanguageToken(LanguageTokenType.AndAnd, "&&", location);
                    }
                    else
                    {
                        MoveNext();
                        return new LanguageToken(LanguageTokenType.And, "&", location);
                    }
                case '|':
                    if (PeekNext(1) == '|')
                    {
                        MoveNext(); MoveNext();
                        location.length = 2;
                        return new LanguageToken(LanguageTokenType.PipePipe, "||", location);
                    }
                    else
                    {
                        MoveNext();
                        return new LanguageToken(LanguageTokenType.Pipe, "|", location);
                    }
            }

            diagnostics.AddFailure("Lexer", "Invalid character in input file '" + current + "'", location, DateTime.Now);
            currChar++;
            return new LanguageToken(LanguageTokenType.BadToken, "", location);
        }

        private char PeekNext(int offset)
        {
            if (currChar + offset >= sourceStore.GetLength())
                return '\0';
            return sourceStore.GetCharAt(currChar + offset);
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