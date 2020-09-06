using System;

namespace SlangLang.Parsing
{
    public enum LanguageTokenType
    {
        //whitespace and control characters
        Whitespace,
        EndOfFile,
        BadToken,

        //literals
        IntegerNumber,
        String,

        //logic operators
        And,
        AndAnd,
        Pipe,
        PipePipe,
        Exclamation,
        ExclamationEquals,
        EqualsEquals,
        Less,
        LessOrEquals,
        Greater,
        GreaterOrEquals,
        
        //maths operations
        Plus,
        Minus,
        Star,
        ForwardSlash,

        //assignment and control
        Semicolon,
        Equals,
        Identifier,
        OpenParanthesis,
        CloseParathesis,
        OpenBrace,
        CloseBrace,

        //basic keywords
        KeywordFalse,
        KeywordTrue,
        KeywordLet,
        KeywordInt,
        KeywordBool,

        //flow control keywords
        KeywordIf,
        KeywordElse,
        KeywordWhile,
    }
}