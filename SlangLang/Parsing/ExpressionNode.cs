using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class ExpressionNode
    {
        public readonly ExpressionNodeType nodeType;
        public readonly TextSpan textLocation;
        public readonly LanguageToken token;

        public ExpressionNode(LanguageToken token, ExpressionNodeType type, TextSpan where)
        {
            nodeType = type;
            textLocation = where;
            this.token = token;
        }

        public abstract List<ExpressionNode> GetChildren();

        public abstract override string ToString();
    }
}