using System;

namespace SlangLang.Parsing
{
    public enum ParseNodeType : int
    {
        UnaryExpression,
        BinaryExpression,
        LiteralExpression,  
        NameExpression,
        AssignmentExpression,
        CallExpression,

        CompilationUnit,

        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,
    }
}