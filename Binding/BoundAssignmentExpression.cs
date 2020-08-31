using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public readonly string name;
        public readonly BoundExpression expression;

        public BoundAssignmentExpression(string variableName, BoundExpression expr, TextLocation where) : base(expr.boundType, BoundNodeType.AssignmentExpression, where)
        {
            name = variableName;
            expression = expr;
        }

        public override List<BoundExpression> GetChildren()
        {
            return new List<BoundExpression>() { expression };
        }

        public override string ToString()
        {
            return "[AssignmentExpression] " + name;
        }
    }
}