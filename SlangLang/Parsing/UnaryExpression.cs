using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class UnaryExpression : ExpressionNode
    {
        public readonly ExpressionNode operand;

        public UnaryExpression(LanguageToken opToken, ExpressionNode node, TextSpan where) : base(opToken, ParseNodeType.Unary, where)
        {
            operand = node;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { operand };
        }
        
        public override string ToString()
        {
            return "[UnaryExpression] " + base.token.tokenType + " (" + base.token.text + ")";
        }
    }
}