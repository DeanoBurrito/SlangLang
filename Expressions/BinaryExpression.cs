using System;
using System.Collections.Generic;

namespace SlangLang.Expressions
{
    public sealed class BinaryExpression : ExpressionNode
    {
        public readonly ExpressionNode leftNode;
        public readonly ExpressionNode rightNode;

        public BinaryExpression(ExpressionNodeType type, ExpressionNode left, ExpressionNode right) : base(type)
        {
            leftNode = left;
            rightNode = right;
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>() { leftNode, rightNode };
        }

        public override string ToString()
        {
            return "[BinaryExpression] " + nodeType.ToString();
        }
    }
}