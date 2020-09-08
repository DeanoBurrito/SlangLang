using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class IfStatement : StatementNode
    {
        public readonly LanguageToken ifKeyword;
        public readonly ExpressionNode condition;
        public readonly StatementNode bodyStatement;
        public readonly ElseClauseData elseClause;
        
        public IfStatement(LanguageToken ifKeyword, ExpressionNode condition, StatementNode thenStatement, ElseClauseData elseClause) 
            : base(ParseNodeType.IfStatement, new TextSpan(ifKeyword.textLocation.start, elseClause == null ? thenStatement.textLocation.end : elseClause.statement.textLocation.end))
        {
            this.ifKeyword = ifKeyword;
            this.condition = condition;
            this.bodyStatement = thenStatement;
            this.elseClause = elseClause;
        }

        public override List<ParseNode> GetChildren()
        {
            if (elseClause == null)
                return new List<ParseNode>() { condition, bodyStatement };
            else
                return new List<ParseNode>() { condition, bodyStatement, elseClause.statement };
        }

        public override string ToString()
        {
            return "[IfStatement]";
        }
    }

    public sealed class ElseClauseData
    {
        public readonly LanguageToken keyword;
        public readonly StatementNode statement;

        public ElseClauseData(LanguageToken elseKeyword, StatementNode elseStatement)
        {
            keyword = elseKeyword;
            statement = elseStatement;
        }
    }
}