using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
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
            StringBuilder sb = new StringBuilder(" (");
            for (int i = 0; i < function.parameters.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(function.parameters[i]);
            }
            sb.Append(")");

            return "[CallExpression] " + function.ToString() + sb.ToString() + ", rtnType=" + function.returnType;
        }
    }
}