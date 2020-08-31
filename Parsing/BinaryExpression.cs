using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class BinaryExpression : ExpressionNode
    {
        public readonly ExpressionNode leftNode;
        public readonly ExpressionNode rightNode;
        public readonly LanguageToken opToken;

        public BinaryExpression(LanguageToken opToken, ExpressionNode left, ExpressionNode right, TextLocation where) : base(ExpressionNodeType.Binary, where)
        {
            leftNode = left;
            rightNode = right;
            this.opToken = opToken;
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>() { leftNode, rightNode };
        }

        public override string ToString()
        {
            return "[BinaryExpression] " + opToken.tokenType + " (" + opToken.text + ")";
        }
    }
}