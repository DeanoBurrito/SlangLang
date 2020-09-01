using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class LiteralExpression : ExpressionNode
    {
        public readonly object value;
        public readonly LiteralValueSpecifier valueSpecifier = LiteralValueSpecifier.Default;

        public LiteralExpression(object val, TextLocation where,LiteralValueSpecifier spec = LiteralValueSpecifier.Default) : base(ExpressionNodeType.Literal, where)
        {
            value = val;
            valueSpecifier = spec;
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>(0);
        }

        public override string ToString()
        {
            string specifierStr = valueSpecifier != LiteralValueSpecifier.Default ? " (" + valueSpecifier.ToString() + ")" : " (*)";
            return "[Literal] " + value.ToString() + specifierStr;
        }
    }
}