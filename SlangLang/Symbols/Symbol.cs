using System;

namespace SlangLang.Symbols
{
    public abstract class Symbol
    {
        public readonly string name;
        public readonly SymbolType symbolType;
        
        private protected Symbol(SymbolType type, string name)
        {
            this.name = name;
            this.symbolType = type;
        }

        public override string ToString()
        {
            return "[" + symbolType + "] " + name;
        }
    }
}