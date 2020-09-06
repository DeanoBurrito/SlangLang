using System;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public readonly VariableSymbol variable;

        public BoundVariableExpression(VariableSymbol variableSymbol, TextSpan where) : base(variableSymbol.type, BoundNodeType.VariableExpression, where)
        {
            variable = variableSymbol;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>();
        }

        public override string ToString()
        {
            return "[VariableExpression] " + variable.name + " (" + variable.type.ToString() + ")";
        }
    }
}