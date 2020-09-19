using System;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Symbols;

namespace SlangLang.Binding
{   
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public readonly VariableSymbol variable;
        public readonly BoundExpression expression;

        public BoundAssignmentExpression(VariableSymbol var, BoundExpression expr, TextSpan where) 
            : base(expr.boundType, BoundNodeType.AssignmentExpression, where)
        {
            variable = var;
            expression = expr;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() { expression };
        }

        public override string ToString()
        {
            return "[AssignmentExpression] " + variable.name + " (" + variable.type + ")";
        }
    }
}