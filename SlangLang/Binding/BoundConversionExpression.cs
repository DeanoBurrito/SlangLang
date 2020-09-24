using System;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public readonly BoundExpression expression;
        
        public BoundConversionExpression(TypeSymbol toType, BoundExpression expression, TextSpan where) : base(toType, BoundNodeType.ConversionExpression, where)
        {
            this.expression = expression;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() { expression };
        }
        
        public override string ToString()
        {
            return "[ConversionExpression] " + expression.boundType + " to " + boundType;
        }
    }
}