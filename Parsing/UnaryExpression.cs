using System;
using System.Collections.Generic;

namespace SlangLang.Parsing
{
    public sealed class UnaryExpression : ExpressionNode
    {
        public readonly ExpressionNode operand;

        public UnaryExpression(ExpressionNodeType type, ExpressionNode node) : base(type) => operand = node;

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>() { operand };
        }
        
        public override string ToString()
        {
            return "[UnaryExpression] " + nodeType.ToString();
        }
    }
}