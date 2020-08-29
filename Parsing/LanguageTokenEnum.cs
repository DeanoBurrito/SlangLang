using System;

namespace SlangLang.Parsing
{
    public enum LanguageTokenType : int
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
        
        //maths operations
        Plus,
        Minus,
        Star,
        ForwardSlash,

        //assignment and control
        Exclamation,
        Semicolon,
        Equals,
    }
}