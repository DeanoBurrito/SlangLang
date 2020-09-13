using System;

namespace SlangLang.Drivers
{
    public sealed class CompilationOptions
    {
        public static CompilationOptions DefaultOptions = new CompilationOptions();
        
        public bool printLexerOutput = false;
        public bool printParserOutput = false;
        public bool printBinderOutput = false;
        public bool printLoweredOutput = false;
    }
}