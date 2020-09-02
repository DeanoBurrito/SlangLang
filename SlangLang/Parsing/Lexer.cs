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
            sourceStore = new TextStore(sourceName, new string[] {sourceLine} );
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
            TextLocation startLocation = sourceStore.GetLocation(start);
            TextLocation endLocation = sourceStore.GetLocation(start + 1); //default ending is only 1 char

            switch (current)
            {
                case '\0':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.EndOfFile, "", new TextSpan(startLocation, endLocation, 1));
                case '+':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Plus, "+", new TextSpan(startLocation, endLocation, 1));
                case '-':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Minus, "-", new TextSpan(startLocation, endLocation, 1));
                case '*':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Star, "*", new TextSpan(startLocation, endLocation, 1));
                case '/':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.ForwardSlash, "/", new TextSpan(startLocation, endLocation, 1));
                case ';':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.Semicolon, ";", new TextSpan(startLocation, endLocation, 1));
                case '(':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.OpenParanthesis, "(", new TextSpan(startLocation, endLocation, 1));
                case ')':
                    currChar++;
                    return new LanguageToken(LanguageTokenType.CloseParathesis, ")", new TextSpan(startLocation, endLocation, 1));
                case '!':
                    if (PeekNext(1) == '=')
                    {
                        currChar += 2;
                        endLocation = sourceStore.GetLocation(currChar);
                        return new LanguageToken(LanguageTokenType.ExclamationEquals, "!=", new TextSpan(startLocation, endLocation, 2));
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.Exclamation, "!", new TextSpan(startLocation, endLocation, 1));
                    }
                case '=':
                    if (PeekNext(1) == '=')
                    {
                        currChar += 2;
                        endLocation = sourceStore.GetLocation(currChar);
                        return new LanguageToken(LanguageTokenType.EqualsEquals, "==", new TextSpan(startLocation, endLocation, 2));
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.Equals, "=", new TextSpan(startLocation, endLocation, 1));
                    }
                case '&':
                    if (PeekNext(1) == '&')
                    {
                        currChar += 2;
                        endLocation = sourceStore.GetLocation(currChar);
                        return new LanguageToken(LanguageTokenType.AndAnd, "&&", new TextSpan(startLocation, endLocation, 2));
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.And, "&", new TextSpan(startLocation, endLocation, 1));
                    }
                case '|':
                    if (PeekNext(1) == '|')
                    {
                        currChar += 2;
                        endLocation = sourceStore.GetLocation(currChar);
                        return new LanguageToken(LanguageTokenType.PipePipe, "||", new TextSpan(startLocation, endLocation, 2));
                    }
                    else
                    {
                        currChar++;
                        return new LanguageToken(LanguageTokenType.Pipe, "|", new TextSpan(startLocation, endLocation, 1));
                    }
                default:
                {
                    int textLen;
                    string text;
                    if (char.IsLetter(current))
                    {
                        //text
                        while (char.IsLetter(current))
                        {
                            current = MoveNext();
                        }

                        endLocation = sourceStore.GetLocation(currChar);
                        textLen = currChar - start;
                        text = sourceStore.GetSubstring(start, currChar - start);
                        LanguageTokenType keyword = LanguageFacts.GetKeyword(text);
                        return new LanguageToken(keyword, text, new TextSpan(startLocation, endLocation, textLen));
                    }
                    else if (char.IsWhiteSpace(current))
                    {
                        //whitespace
                        while (char.IsWhiteSpace(current))
                        {
                            current = MoveNext();
                        }

                        endLocation = sourceStore.GetLocation(currChar);
                        textLen = currChar - start;
                        text = sourceStore.GetSubstring(start, currChar - start);
                        return new LanguageToken(LanguageTokenType.Whitespace, text, new TextSpan(startLocation, endLocation, textLen));
                    }
                    else if (char.IsDigit(current))
                    {
                        //some kind of number
                        while (char.IsDigit(current))
                        {
                            current = MoveNext();
                        }

                        endLocation = sourceStore.GetLocation(currChar);
                        textLen = currChar - start;
                        text = sourceStore.GetSubstring(start, currChar - start);
                        return new LanguageToken(LanguageTokenType.IntegerNumber, text, new TextSpan(startLocation, endLocation, textLen));
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
                                diagnostics.AddFailure("Lexer", "Expected \" to end string literal, found end of file.", startLocation, DateTime.Now);
                                return new LanguageToken(LanguageTokenType.EndOfFile, "", new TextSpan(startLocation));
                            }
                        }
                        currChar++;
                        
                        endLocation = sourceStore.GetLocation(currChar);
                        textLen = currChar - start;
                        text = sourceStore.GetSubstring(start, currChar - start);
                        return new LanguageToken(LanguageTokenType.String, text, new TextSpan(startLocation, endLocation, textLen));
                    }

                    diagnostics.AddFailure("Lexer", "Invalid character in input file '" + current + "'", startLocation, DateTime.Now);
                    currChar++;
                    return new LanguageToken(LanguageTokenType.BadToken, "", new TextSpan(startLocation));
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