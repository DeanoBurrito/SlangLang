using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SlangLang.Debug;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public readonly FunctionSymbol function;
        public readonly ImmutableArray<BoundExpression> arguments;

        public BoundCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments, TextSpan where)
            : base(function.returnType, BoundNodeType.CallExpression, where)
        {
            this.function = function;
            this.arguments = arguments;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>(arguments);
        }

        public override string ToString()
        {
            string argList = "";
            foreach (ParameterSymbol p in function.parameters)
            {
                argList += ", " + p.ToString();
            }
            return "[Call] " + function.ToString() + argList;
        }
    }
}