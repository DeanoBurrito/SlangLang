using System;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public readonly VariableSymbol variable;
        public readonly BoundExpression expression;

        public BoundAssignmentExpression(VariableSymbol var, BoundExpression expr, TextSpan where) : base(expr.boundType, BoundNodeType.AssignmentExpression, where)
        {
            variable = var;
            expression = expr;
        }

        public override List<BoundExpression> GetChildren()
        {
            return new List<BoundExpression>() { expression };
        }

        public override string ToString()
        {
            return "[AssignmentExpression] " + variable.name + " (" + variable.type + ")";
        }
    }
}