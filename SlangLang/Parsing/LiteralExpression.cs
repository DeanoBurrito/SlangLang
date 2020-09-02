using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class LiteralExpression : ExpressionNode
    {
        public readonly object value;
        public readonly LiteralValueSpecifier valueSpecifier = LiteralValueSpecifier.Default;

        public LiteralExpression(object val, LanguageToken token, TextSpan where, LiteralValueSpecifier spec = LiteralValueSpecifier.Default) : base(token, ExpressionNodeType.Literal, where)
        {
            value = val;
            valueSpecifier = spec;
        }

        public override string ToString()
        {
            string specifierStr = valueSpecifier != LiteralValueSpecifier.Default ? " (" + valueSpecifier.ToString() + ")" : " (*)";
            return "[Literal] " + value.ToString() + specifierStr;
        }
    }
}