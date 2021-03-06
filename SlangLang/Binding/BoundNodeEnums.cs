using System;

namespace SlangLang.Binding
{
    internal enum BoundNodeType
    {
        ErrorExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        CallExpression,
        ConversionExpression,

        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        LabelStatement,
        GotoStatement,
        ConditionalGotoStatement,
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