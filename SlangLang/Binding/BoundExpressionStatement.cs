using System;
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
    }
}