using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public readonly object value;

        public BoundLiteralExpression(object val, TextSpan where) : base(val.GetType(), BoundNodeType.LiteralExpression, where)
        {
            value = val;
        }

        public override string ToString()
        {
            return "[Literal] " + value + " (" + base.boundType.ToString() + ")";
        }
    }
}