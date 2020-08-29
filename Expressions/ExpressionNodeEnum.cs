using System;

namespace SlangLang.Expressions
{
    public enum ExpressionNodeType : int
    {
        Nop,

        Add,
        Sub,
        Mult,
        Div,

        BooleanLiteral,
        IntegerLiteral,
        FloatLiteral,
        StringLiteral,

        Parenthesized,
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