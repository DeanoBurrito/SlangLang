using System;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class CompilationUnit : ExpressionNode
    {
        public readonly ExpressionNode expression;
        public readonly LanguageToken eofToken;
        
        public CompilationUnit(ExpressionNode expr, LanguageToken eof, TextSpan where) : base(eof, ExpressionNodeType.CompilationUnit, where)
        {
            expression = expr;
            eofToken = eof;
        }

        public override string ToString()
        {
            return "[CompilationUnit]";
        }
    }
}