using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class LanguageToken : ParseNode
    {
        public LanguageTokenType tokenType;
        public string text;

        public LanguageToken(LanguageTokenType type, string txt, TextSpan location) : base(ParseNodeType.LanguageToken, location)
        {
            tokenType = type;
            text = txt;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>();
        }

        public override string ToString()
        {
            return "[LanguageToken: " + tokenType.ToString() + " \"" + text + "\"]";
        }
    }
}