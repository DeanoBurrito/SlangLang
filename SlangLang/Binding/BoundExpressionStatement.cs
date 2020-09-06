using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public readonly BoundExpression expression;

        public BoundExpressionStatement(BoundExpression expr, TextSpan where) : base(BoundNodeType.ExpressionStatement, where)
        {
            expression = expr;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() {expression};
        }

        public override string ToString()
        {
            return "[ExpressionStatement]";
        }
    }
}