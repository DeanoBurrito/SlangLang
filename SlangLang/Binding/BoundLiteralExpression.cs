using System;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public readonly object value;

        public BoundLiteralExpression(object val, TypeSymbol type, TextSpan where) 
            : base(type, BoundNodeType.LiteralExpression, where)
        {
            value = val;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>();
        }

        public override string ToString()
        {
            return "[Literal] " + value + " (" + base.boundType.ToString() + ")";
        }
    }
}