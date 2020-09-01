using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class UnaryExpression : ExpressionNode
    {
        public readonly ExpressionNode operand;

        public UnaryExpression(LanguageToken opToken, ExpressionNode node, TextLocation where) : base(opToken, ExpressionNodeType.Unary, where)
        {
            operand = node;
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>() { operand };
        }
        
        public override string ToString()
        {
            return "[UnaryExpression] " + base.token.tokenType + " (" + base.token.text + ")";
        }
    }
}