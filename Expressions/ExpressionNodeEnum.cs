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

        IntegerLiteral,
        FloatLiteral,
        StringLiteral,
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