using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class ForStatement : StatementNode
    {
        public readonly LanguageToken forKeyword;
        public readonly StatementNode setupStatement;
        public readonly ExpressionNode condition;
        public readonly StatementNode postStatement;
        public readonly StatementNode body;
        
        public ForStatement(LanguageToken keyword, StatementNode setup, ExpressionNode condition, StatementNode post, StatementNode body) 
            : base(ParseNodeType.ForStatement, new TextSpan(keyword.textLocation.start, body.textLocation.end))
        {
            this.forKeyword = keyword;
            this.setupStatement = setup;
            this.condition = condition;
            this.postStatement = post;
            this.body = body;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>(){ setupStatement, condition, postStatement, body };
        }

        public override string ToString()
        {
            return "[ForStatement]";
        }
    }
}