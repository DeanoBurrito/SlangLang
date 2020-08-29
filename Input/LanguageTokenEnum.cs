using System;

namespace SlangLang.Input
{
    public enum LanguageTokenType : int
    {
        Whitespace,
        EndOfFile,
        BadToken,

        IntegerNumber,
        FloatingNumber,
        String,
        
        Plus,
        Minus,
        Star,
        ForwardSlash,

        Exclamation,
        Semicolon,
        Equals,

        OpenParanthesis,
        CloseParathesis,
    }
}