using System;

namespace SlangLang.Binding
{
    internal enum BoundNodeType
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
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
        NotEquals
    }
}