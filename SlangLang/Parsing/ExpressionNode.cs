using System;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class ExpressionNode : ParseNode
    {
        public readonly LanguageToken token;

        public ExpressionNode(LanguageToken token, ParseNodeType type, TextSpan where) : base(type, where)
        {
            this.token = token;
        }

        public override string ToString()
        {
            return token.value;
        }
    }
}