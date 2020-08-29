using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class ExpressionNode
    {
        public readonly ExpressionNodeType nodeType;
        public readonly TextLocation textLocation = TextLocation.NoLocation;

        public ExpressionNode(ExpressionNodeType type)
        {
            nodeType = type;
        }

        public abstract List<ExpressionNode> GetChildren();

        public abstract override string ToString();
    }
}