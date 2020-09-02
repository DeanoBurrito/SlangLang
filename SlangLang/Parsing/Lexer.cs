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
            char current = PeekNext(0);
            int start = currChar;
            TextLocation location = sourceStore.GetLocation(start);
            location.filename = filename;
            location.length = 1;

            switch (current)
            {
                case '\0':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.EndOfFile, "", new TextLocation(filename, 0, 0));
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
                case ';':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Semicolon, ";", location);
                case '(':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.OpenParanthesis, "(", location);
                case ')':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.CloseParathesis, ")", location);
                case '!':
                    if (PeekNext(1) == '=')
                    {
                        currChar += 2;
                        location.length = 2;
                        return new LanguageToken(LanguageTokenType.ExclamationEquals, "!=", location);
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.Exclamation, "!", location);
                    }
                case '=':
                    if (PeekNext(1) == '=')
                    {
                        currChar += 2;
                        location.length = 2;
                        return new LanguageToken(LanguageTokenType.EqualsEquals, "==", location);
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.Equals, "=", location);
                    }
                case '&':
                    if (PeekNext(1) == '&')
                    {
                        currChar += 2;
                        location.length = 2;
                        return new LanguageToken(LanguageTokenType.AndAnd, "&&", location);
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.And, "&", location);
                    }
                case '|':
                    if (PeekNext(1) == '|')
                    {
                        currChar += 2;
                        location.length = 2;
                        return new LanguageToken(LanguageTokenType.PipePipe, "||", location);
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.Pipe, "|", location);
                    }
                default:
                {
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
                    else if (char.IsWhiteSpace(current))
                    {
                        //whitespace
                        while (char.IsWhiteSpace(current))
                        {
                            current = MoveNext();
                        }
                        location.length = currChar - start;
                        return new LanguageToken(LanguageTokenType.Whitespace, sourceStore.GetSubstring(start, currChar - start), location);
                    }
                    else if (char.IsDigit(current))
                    {
                        //some kind of number
                        while (char.IsDigit(current))
                        {
                            current = MoveNext();
                        }
                        location.length = currChar - start;
                        return new LanguageToken(LanguageTokenType.IntegerNumber, sourceStore.GetSubstring(start, currChar - start), location);
                    }
                    else if (current == '"')
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
                        currChar++;
                        location.length = currChar - start;
                        return new LanguageToken(LanguageTokenType.String, sourceStore.GetSubstring(start, currChar - start), location);
                    }

                    diagnostics.AddFailure("Lexer", "Invalid character in input file '" + current + "'", location, DateTime.Now);
                    currChar++;
                    return new LanguageToken(LanguageTokenType.BadToken, "", location);
                }
            } 
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