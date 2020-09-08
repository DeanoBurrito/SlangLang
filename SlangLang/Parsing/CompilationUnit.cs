using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class CompilationUnit : ParseNode
    {
        public readonly StatementNode statement;
        public readonly LanguageToken eofToken;
        
        public CompilationUnit(StatementNode statement, LanguageToken eof) : base(ParseNodeType.CompilationUnit, statement.textLocation)
        {
            this.statement = statement;
            eofToken = eof;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { statement };
        }

        public override string ToString()
        {
            return "[CompilationUnit]";
        }
    }
}