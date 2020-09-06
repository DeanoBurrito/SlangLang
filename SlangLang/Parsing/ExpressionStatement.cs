using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class ExpressionStatement : StatementNode
    {
        public readonly ExpressionNode expression;
        public readonly LanguageToken semicolonToken;
        
        public ExpressionStatement(ExpressionNode expr, LanguageToken semicolon) : base(expr.textLocation, ParseNodeType.ExpressionStatement)
        {
            expression = expr;
            semicolonToken = semicolon;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { expression };
        }

        public override string ToString()
        {
            return "[ExpressionStatement]";
        }
    }
}