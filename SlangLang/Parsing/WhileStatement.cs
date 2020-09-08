using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class WhileStatement : StatementNode
    {
        public readonly LanguageToken whileKeyword;
        public readonly ExpressionNode condition;
        public readonly StatementNode body;

        public WhileStatement(LanguageToken keyword, ExpressionNode condition, StatementNode body) 
            : base(ParseNodeType.WhileStatement, new TextSpan(keyword.textLocation.start, body.textLocation.end))
        {
            whileKeyword = keyword;
            this.condition = condition;
            this.body = body;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { condition, body };
        }

        public override string ToString()
        {
            return "[WhileStatement]";
        }
    }
}