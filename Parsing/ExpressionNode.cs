using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class ExpressionNode
    {
        public readonly ExpressionNodeType nodeType;
        public readonly TextLocation textLocation;

        public ExpressionNode(ExpressionNodeType type, TextLocation where)
        {
            nodeType = type;
            textLocation = where;
        }

        public abstract List<ExpressionNode> GetChildren();

        public abstract override string ToString();
    }
}