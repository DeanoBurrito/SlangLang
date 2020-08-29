using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public readonly BoundExpression left;
        public readonly BoundExpression right;
        public readonly BoundBinaryOperatorType operatorType;

        public BoundBinaryExpression(BoundBinaryOperatorType operatorType, BoundExpression left, BoundExpression right, TextLocation where) 
            : base(left.boundType, BoundNodeType.BinaryExpression, where)
        {
            this.operatorType = operatorType;
            this.left = left;
            this.right = right;
        }

        public override List<BoundExpression> GetChildren()
        {
            return new List<BoundExpression>() { left, right };
        }

        public override string ToString()
        {
            return "[BinaryExpression] " + operatorType;
        }
    }
}