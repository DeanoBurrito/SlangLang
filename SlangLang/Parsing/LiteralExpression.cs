using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class LiteralExpression : ExpressionNode
    {
        public readonly object value;

        public LiteralExpression(object val, LanguageToken token, TextSpan where) : base(token, ParseNodeType.Literal, where)
        {
            value = val;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>();
        }

        public override string ToString()
        {
            return "[Literal] " + value.ToString();
        }
    }
}