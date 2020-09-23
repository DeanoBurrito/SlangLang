using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class LanguageToken : ParseNode
    {
        public LanguageTokenType tokenType;
        public string value;
        public string text;

        public LanguageToken(LanguageTokenType type, string value, string srcText, TextSpan location) : base(ParseNodeType.LanguageToken, location)
        {
            tokenType = type;
            this.value = value;
            text = srcText;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>();
        }

        public override string ToString()
        {
            return "[LanguageToken] type=" + tokenType + ", val=" + value + ", text=" + text;
        }
    }
}