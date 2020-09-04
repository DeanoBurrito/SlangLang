using System;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal abstract class BoundNode
    {
        public readonly BoundNodeType nodeType;
        public readonly TextSpan textLocation;

        public BoundNode(BoundNodeType type, TextSpan where)
        {
            nodeType = type;
            textLocation = where;
        }
    }
}