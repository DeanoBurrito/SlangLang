using System;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class LanguageToken
    {
        public TextSpan textLocation;
        public LanguageTokenType tokenType;
        public string text;

        public LanguageToken(LanguageTokenType type, string txt, TextSpan location)
        {
            tokenType = type;
            textLocation = location;
            text = txt;
        }

        public override string ToString()
        {
            return "[LanguageToken: " + tokenType.ToString() + " \"" + text + "\"]";
        }
    }
}