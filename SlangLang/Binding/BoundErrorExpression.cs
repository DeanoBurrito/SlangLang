using System;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public BoundErrorExpression(TextSpan where) : base(TypeSymbol.Error, BoundNodeType.ErrorExpression, where)
        {}

        public override List<BoundNode> GetChildren() => new List<BoundNode>();

        public override string ToString()
        {
            return "[ErrorExpression]";
        }
    }
}