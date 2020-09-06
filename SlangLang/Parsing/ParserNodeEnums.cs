using System;

namespace SlangLang.Parsing
{
    public enum ParseNodeType : int
    {
        Unary,
        Binary,
        Literal,  
        Name,
        Assignment,

        CompilationUnit,

        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
    }
}