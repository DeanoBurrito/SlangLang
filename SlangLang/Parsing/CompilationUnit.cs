using System;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class CompilationUnit : ParseNode
    {
        public readonly StatementNode statement;
        public readonly LanguageToken eofToken;
        
        public CompilationUnit(StatementNode statement, LanguageToken eof, TextSpan where) : base(ParseNodeType.CompilationUnit, where)
        {
            this.statement = statement;
            eofToken = eof;
        }

        public override string ToString()
        {
            return "[CompilationUnit]";
        }
    }
}