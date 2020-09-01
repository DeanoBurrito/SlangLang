using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class UnaryExpression : ExpressionNode
    {
        public readonly ExpressionNode operand;
        public readonly LanguageToken opToken;

        public UnaryExpression(LanguageToken opToken, ExpressionNode node, TextLocation where) : base(ExpressionNodeType.Unary, where)
        {
            operand = node;
            this.opToken = opToken;
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>() { operand };
        }
        
        public override string ToString()
        {
            return "[UnaryExpression] " + opToken.tokenType + " (" + opToken.text + ")";
        }
    }
}