using System;
using System.Collections.Generic;
using SlangLang.Debugging;

namespace SlangLang.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public readonly BoundExpression operand;
        public readonly BoundUnaryOperatorType operatorType;

        public BoundUnaryExpression(BoundUnaryOperatorType operatorType, BoundExpression inner, TextLocation where) : base(inner.boundType, BoundNodeType.UnaryExpression, where)
        {
            operand = inner;
            this.operatorType = operatorType;
        }

        public override List<BoundExpression> GetChildren()
        {
            return new List<BoundExpression>() { operand };
        }

        public override string ToString()
        {
            return "[UnaryExpression] " + operatorType;
        }
    }
}