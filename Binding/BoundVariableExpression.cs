using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public readonly string name;

        public BoundVariableExpression(string variableName, Type variableType, TextLocation where) : base(variableType, BoundNodeType.VariableExpression, where)
        {
            name = variableName;
        }

        public override List<BoundExpression> GetChildren()
        {
            return new List<BoundExpression>();
        }

        public override string ToString()
        {
            return "[VariableExpression] " + name + " (" + base.boundType.ToString() + ")";
        }
    }
}