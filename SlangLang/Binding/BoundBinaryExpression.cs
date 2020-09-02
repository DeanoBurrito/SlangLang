using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public readonly BoundExpression left;
        public readonly BoundExpression right;
        public readonly BoundBinaryOperator op;

        public BoundBinaryExpression(BoundBinaryOperator op, BoundExpression left, BoundExpression right, TextSpan where) 
            : base(op.resultType, BoundNodeType.BinaryExpression, where)
        {
            this.op = op;
            this.left = left;
            this.right = right;
        }

        public override string ToString()
        {
            return "[BinaryExpression] " + op;
        }
    }
}