using System;
using SlangLang.Debug;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public readonly TypeSymbol boundType;
        
        public BoundExpression(TypeSymbol bindingType, BoundNodeType nodeType, TextSpan where) : base(nodeType, where)
        {
            boundType = bindingType;
        }
    }
}