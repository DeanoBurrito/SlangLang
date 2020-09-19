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
        Circumflex,

        //assignment and control
        Semicolon,
        Equals,
        Identifier,
        OpenParanthesis,
        CloseParanthesis,
        OpenBrace,
        CloseBrace,

        //basic keywords
        KeywordFalse,
        KeywordTrue,
        KeywordLet,
        KeywordInt,
        KeywordBool,
        KeywordString,

        //flow control keywords
        KeywordIf,
        KeywordElse,
        KeywordWhile,
        KeywordFor,
    }
}