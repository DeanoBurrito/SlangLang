using System;
using System.Collections.Generic;

namespace SlangLang.Expressions
{
    public sealed class UnaryExpression : ExpressionNode
    {
        public ExpressionNode child;

        public UnaryExpression(ExpressionNodeType type, ExpressionNode node) : base(type) => child = node;

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>() { child };
        }
        
        public override string ToString()
        {
            return "[UnaryExpression] " + nodeType.ToString();
        }
    }
}