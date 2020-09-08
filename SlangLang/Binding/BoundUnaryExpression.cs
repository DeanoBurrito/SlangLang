using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public readonly BoundExpression operand;
        public readonly BoundUnaryOperator op;

        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression inner, TextSpan where) 
            : base(op.resultType, BoundNodeType.UnaryExpression, where)
        {
            operand = inner;
            this.op = op;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() { operand };
        }

        public override string ToString()
        {
            return "[UnaryExpression] " + op;
        }
    }
}