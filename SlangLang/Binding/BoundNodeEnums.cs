using System;

namespace SlangLang.Binding
{
    internal enum BoundNodeType
    {
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        LabelStatement,
        GotoStatement,
        ConditionalGotoStatement,
        
        ErrorExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
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

        BitwiseOr,
        ConditionalOr,
        BitwiseAnd,
        ConditionalAnd,
        BitwiseXor,

        Equals,
        NotEquals,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
    }
}