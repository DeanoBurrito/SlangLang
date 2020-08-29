using System;

namespace SlangLang.Parsing
{
    public enum ExpressionNodeType : int
    {
        Nop,

        //Unary Operators
        Negate,

        //Binary Operators
        Add,
        Sub,
        Mult,
        Div,

        //Literals
        BooleanLiteral,
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