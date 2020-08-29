using System;

namespace SlangLang.Parsing
{
    public enum LanguageTokenType
    {
        //whitespace and control characters
        Whitespace,
        EndOfFile,
        BadToken,
        OpenParanthesis,
        CloseParathesis,

        //literals
        IntegerNumber,
        String,

        //logic operators
        And,
        AndAnd,
        Pipe,
        PipePipe,
        Exclamation,
        
        //maths operations
        Plus,
        Minus,
        Star,
        ForwardSlash,

        //assignment and control
        Semicolon,
        Equals,
        EqualsEquals,
        Identifier,

        //keywords
        KeywordFalse,
        KeywordTrue,
    }
}