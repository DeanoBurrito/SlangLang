using System;
using SlangLang.Debugging;

namespace SlangLang.Input
{
    public sealed class LanguageToken
    {
        public TextLocation sourceLocation;
        public LanguageTokenType tokenType;
        public string text;

        public LanguageToken(LanguageTokenType type, string txt, TextLocation srcLocation)
        {
            tokenType = type;
            sourceLocation = srcLocation;
            text = txt;
        }

        public override string ToString()
        {
            return "[LanguageToken: " + tokenType.ToString() + " \"" + text + "\"]";
        }
    }
}