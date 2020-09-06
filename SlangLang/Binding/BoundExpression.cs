using System;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public readonly Type boundType;
        
        public BoundExpression(Type bindingType, BoundNodeType nodeType, TextSpan where) :base(nodeType, where)
        {
            boundType = bindingType;
        }
    }
}