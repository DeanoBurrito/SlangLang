using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class BinaryExpression : ExpressionNode
    {
        public readonly ExpressionNode leftNode;
        public readonly ExpressionNode rightNode;

        public BinaryExpression(LanguageToken opToken, ExpressionNode left, ExpressionNode right, TextSpan where) : base(opToken, ExpressionNodeType.Binary, where)
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
            return "[BinaryExpression] " + base.token.tokenType + " (" + base.token.text + ")";
        }
    }
}