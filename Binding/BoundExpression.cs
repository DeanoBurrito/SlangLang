using System;
using System.Collections.Generic;
using SlangLang.Debugging;

namespace SlangLang.Binding
{
    internal abstract class BoundExpression
    {
        public readonly Type boundType;
        public readonly BoundNodeType nodeType;
        public readonly TextLocation textLocation;
        
        public BoundExpression(Type bindingType, BoundNodeType nodeType, TextLocation where)
        {
            boundType = bindingType;
            this.nodeType = nodeType;
            textLocation = where;
        }

        public abstract List<BoundExpression> GetChildren();

        public abstract override string ToString();
    }
}