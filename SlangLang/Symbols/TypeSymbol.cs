using System;

namespace SlangLang.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new TypeSymbol("?");
        public static readonly TypeSymbol Int = new TypeSymbol("Int32");
        public static readonly TypeSymbol Bool = new TypeSymbol("Bool");
        public static readonly TypeSymbol String = new TypeSymbol("String");
        
        internal TypeSymbol(string name) : base(SymbolType.Type, name)
        {
        }

        public override string ToString()
        {
            return base.name;
        }
    }
}