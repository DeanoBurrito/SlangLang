using System;

namespace SlangLang.Parsing
{
    public enum ExpressionNodeType : int
    {
        Nop,

        //Unary Operators
        Negate,
        Not,

        //Binary Operators
        Addition,
        Subtraction,
        Multiplication,
        Division,
        ConditionalOr,
        ConditionalAnd,

        //Literals
        Literal,
    }

    public enum LiteralValueSpecifier
    {
        Default,

        U8,
        U16,
        U32,
        U64,
        S8,
        S16,
        S32,
        S64,

        SinglePrecision,
        DoublePrecision,
    }
}