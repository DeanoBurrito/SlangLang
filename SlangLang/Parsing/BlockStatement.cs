using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class BlockStatement : StatementNode
    {
        public readonly LanguageToken openBraceToken;
        public readonly ImmutableArray<StatementNode> statements;
        public readonly LanguageToken closeBraceToken;
        
        public BlockStatement(LanguageToken openBrace, ImmutableArray<StatementNode> statements, LanguageToken closeBrace) 
            : base (new TextSpan(openBrace.textLocation.start, closeBrace.textLocation.end), ParseNodeType.BlockStatement)
        {
            openBraceToken = openBrace;
            closeBraceToken = closeBrace;
            this.statements = statements;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>(statements);
        }

        public override string ToString()
        {
            return "[BlockStatement]";
        }
    }
}