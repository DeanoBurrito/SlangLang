using System;
using System.Collections.Immutable;

namespace SlangLang.Symbols
{
    public sealed class ParameterSymbol : VariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type) : base(name, true, type, SymbolType.Parameter)
        {
            
        }
    }
}