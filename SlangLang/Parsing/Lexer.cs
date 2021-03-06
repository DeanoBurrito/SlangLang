using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class Lexer
    {
        readonly TextStore sourceStore;
        int currChar;

        char current;
        int start;
        TextLocation startLocation;
        TextLocation endLocation;
        string tokenText;
        string tokenValue;
        int textLength;
        LanguageTokenType type;

        readonly Diagnostics diagnostics;

        public Lexer(Diagnostics diag, TextStore source)
        {
            diagnostics = diag;
            sourceStore = source;
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
            current = PeekNext(0);
            start = currChar;
            startLocation = sourceStore.GetLocation(start);
            endLocation = sourceStore.GetLocation(start + 1); //default ending is only 1 char
            tokenText = null;
            tokenValue = null;
            textLength = 1;
            type = LanguageTokenType.BadToken;

            switch (current)
            {
                case '\0':
                    currChar++;
                    type = LanguageTokenType.EndOfFile;
                    break;
                case '+':
                    currChar++;
                    type = LanguageTokenType.Plus;
                    break;
                case '-':
                    currChar++;
                    type = LanguageTokenType.Minus;
                    break;
                case '*':
                    currChar++;
                    type = LanguageTokenType.Star;
                    break;
                case '/':
                    currChar++;
                    type = LanguageTokenType.ForwardSlash;
                    break;
                case ';':
                    currChar++;
                    type = LanguageTokenType.Semicolon;
                    break;
                case '(':
                    currChar++;
                    type = LanguageTokenType.OpenParanthesis;
                    break;
                case ')':
                    currChar++;
                    type = LanguageTokenType.CloseParanthesis;
                    break;
                case '{':
                    currChar++;
                    type = LanguageTokenType.OpenBrace;
                    break;
                case '}':
                    currChar++;
                    type = LanguageTokenType.CloseBrace;
                    break;
                case ',':
                    currChar++;
                    type = LanguageTokenType.Comma;
                    break;
                case '^':
                    currChar++;
                    type = LanguageTokenType.Circumflex;
                    break;
                case '!':
                    currChar++;
                    if (PeekNext(0) == '=')
                    {
                        ExtendTokenEnd();
                        type = LanguageTokenType.ExclamationEquals;
                    }
                    else
                    {
                        type = LanguageTokenType.Exclamation;
                    }
                    break;
                case '=':
                    currChar++;
                    if (PeekNext(0) == '=')
                    {
                        ExtendTokenEnd();
                        type = LanguageTokenType.EqualsEquals;
                    }
                    else
                    {
                        type =LanguageTokenType.Equals;
                    }
                    break;
                case '&':
                    currChar++;
                    if (PeekNext(0) == '&')
                    {
                        ExtendTokenEnd();
                        type = LanguageTokenType.AndAnd;
                    }
                    else
                    {
                        type = LanguageTokenType.And;
                    }
                    break;
                case '|':
                    currChar++;
                    if (PeekNext(0) == '|')
                    {
                        ExtendTokenEnd();
                        type = LanguageTokenType.PipePipe;
                    }
                    else
                    {
                        type = LanguageTokenType.Pipe;
                    }
                    break;
                case '<':
                    currChar++;
                    if (PeekNext(0) == '=')
                    {
                        ExtendTokenEnd();
                        type = LanguageTokenType.LessOrEquals;
                    }
                    else
                    {
                        type = LanguageTokenType.Less;
                    }
                    break;
                case '>':
                    currChar++;
                    if (PeekNext(0) == '=')
                    {
                        ExtendTokenEnd();
                        type = LanguageTokenType.GreaterOrEquals;
                    }
                    else
                    {
                        type = LanguageTokenType.Greater;
                    }
                    break;
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    ReadWhitespaceToken();
                    break;
                default:
                {
                    if (char.IsLetter(current))
                    {
                        ReadKeywordIdentifier();
                    }
                    else if (char.IsWhiteSpace(current))
                    {
                        ReadWhitespaceToken();
                    }
                    else if (char.IsDigit(current))
                    {
                        ReadNumberToken();
                    }
                    else if (current == '"')
                    {
                        ReadStringToken();
                    }
                    else
                    {
                        currChar++;
                        diagnostics.LexerError_GotBadInput(current.ToString(), new TextSpan(startLocation, endLocation));
                    }
                    break;
                }
            }

            if (tokenText == null)
                tokenText = LanguageFacts.GetText(type);
            if (tokenText == null && type != LanguageTokenType.BadToken && type != LanguageTokenType.EndOfFile)
                throw new Exception("Unexpected null text, text did not trigger bad token, but was not lexable as any known token.");

            if (tokenValue == null)
                tokenValue = tokenText;
            return new LanguageToken(type, tokenValue, tokenText, new TextSpan(startLocation, endLocation, textLength));
        }

        private void ExtendTokenEnd()
        {
            endLocation = sourceStore.GetLocation(currChar);
            textLength++;
            currChar++;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(current))
            {
                current = MoveNext();
            }

            endLocation = sourceStore.GetLocation(currChar);
            textLength = currChar - start;
            tokenText = sourceStore.GetSubstring(start, currChar - start);
            type = LanguageTokenType.IntegerNumber;
        }

        private void ReadWhitespaceToken()
        {
            while (char.IsWhiteSpace(current))
            {
                current = MoveNext();
            }

            endLocation = sourceStore.GetLocation(currChar);
            textLength = currChar - start;
            tokenText = sourceStore.GetSubstring(start, currChar - start);
            type = LanguageTokenType.Whitespace;
        }

        private void ReadKeywordIdentifier()
        {
            while (char.IsLetter(current))
            {
                current = MoveNext();
            }

            endLocation = sourceStore.GetLocation(currChar);
            textLength = currChar - start;
            tokenText = sourceStore.GetSubstring(start, currChar - start);
            type = LanguageFacts.GetKeyword(tokenText);
        }

        private void ReadStringToken()
        {
            current = MoveNext();
            while (current != '"')
            {
                current = MoveNext();
                if (current == '\0')
                {
                    diagnostics.LexerError_ExpectedEndOfStringLiteral("EOF", new TextSpan(startLocation, endLocation));

                    endLocation = TextLocation.NoLocation;
                    textLength = 0;
                    tokenText = sourceStore.GetSubstring(start, currChar - start);
                    type = LanguageTokenType.EndOfFile;
                }
            }
            currChar++;
            
            endLocation = sourceStore.GetLocation(currChar);
            textLength = currChar - start;
            tokenText = sourceStore.GetSubstring(start, textLength);
            tokenValue = tokenText.Substring(1, textLength - 2);
            type = LanguageTokenType.String;
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