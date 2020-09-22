using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class BinaryExpression : ExpressionNode
    {
        public readonly ExpressionNode leftNode;
        public readonly ExpressionNode rightNode;

        public BinaryExpression(ExpressionNode left, LanguageToken opToken, ExpressionNode right) 
            : base(opToken, ParseNodeType.BinaryExpression, new TextSpan(left.textLocation.start, right.textLocation.end))
        {
            leftNode = left;
            rightNode = right;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { leftNode, rightNode };
        }

        public override string ToString()
        {
            return "[BinaryExpression] " + base.token.tokenType + " (" + base.token.text + ")";
        }
    }
}