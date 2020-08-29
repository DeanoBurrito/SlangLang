using System;
using System.Collections.Generic;

namespace SlangLang.Expressions
{
    public sealed class LiteralExpression : ExpressionNode
    {
        public readonly object value;
        public readonly LiteralValueSpecifier valueSpecifier = LiteralValueSpecifier.Default;

        public LiteralExpression(ExpressionNodeType type, object val, LiteralValueSpecifier spec = LiteralValueSpecifier.Default) : base(type)
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
            return "[Literal] " + nodeType.ToString().Replace("Literal", "") + "=" + value.ToString() + specifierStr;
        }
    }
}