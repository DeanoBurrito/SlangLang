using System;

namespace SlangLang.Binding
{
    internal enum BoundNodeType
    {
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
        IfStatement,
        WhileStatement,
        ForStatement,
    }

    internal enum BoundUnaryOperatorType
    {
        Negate,
        Not,
    }

    internal enum BoundBinaryOperatorType
    {
        Addition,
        Subtract,
        Multiplication,
        Division,

        ConditionalOr,
        ConditionalAnd,

        Equals,
        NotEquals,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
    }
}