using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public readonly BoundExpression operand;
        public readonly BoundUnaryOperator op;

        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression inner, TextLocation where) : base(inner.boundType, BoundNodeType.UnaryExpression, where)
        {
            operand = inner;
            this.op = op;
        }

        public override List<BoundExpression> GetChildren()
        {
            return new List<BoundExpression>() { operand };
        }

        public override string ToString()
        {
            return "[UnaryExpression] " + op;
        }
    }
}