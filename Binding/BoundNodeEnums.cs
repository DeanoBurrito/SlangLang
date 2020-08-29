using System;

namespace SlangLang.Binding
{
    internal enum BoundNodeType
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
    }

    internal enum BoundUnaryOperatorType
    {
        Negate,
    }

    internal enum BoundBinaryOperatorType
    {
        Addition,
        Subtract,
        Multiplication,
        Division,
    }
}