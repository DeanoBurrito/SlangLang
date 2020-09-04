using System;
using System.Collections.Immutable;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal abstract class BoundStatement : BoundNode
    {
        public BoundStatement(BoundNodeType type, TextSpan where) : base(type, where)
        {}
    }

    internal sealed class BoundBlockStatement : BoundStatement
    {
        public readonly ImmutableArray<BoundStatement> statements;
        
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements, TextSpan where) : base (BoundNodeType.BlockStatement, where)
        {
            this.statements = statements;
        }
    }

    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public readonly BoundExpression expression;

        public BoundExpressionStatement(BoundExpression expr, TextSpan where) : base(BoundNodeType.ExpressionStatement, where)
        {
            expression = expr;
        }
    }
}