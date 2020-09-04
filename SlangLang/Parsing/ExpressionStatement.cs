using System;
using System.Collections.Immutable;
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
    }
}