using System;
using System.Collections.Generic;

namespace SlangLang.Expressions
{
    public abstract class ExpressionNode
    {
        public readonly ExpressionNodeType nodeType;

        public ExpressionNode(ExpressionNodeType type)
        {
            nodeType = type;
        }

        public abstract List<ExpressionNode> GetChildren();

        public abstract override string ToString();
    }
}