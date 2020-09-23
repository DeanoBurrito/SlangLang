using System;
using System.Collections.Immutable;

namespace SlangLang.Symbols
{    
    public sealed class FunctionSymbol : Symbol
    {
        public readonly ImmutableArray<ParameterSymbol> parameters;
        public readonly TypeSymbol returnType;
        
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType) : base(SymbolType.Function, name)
        {   
            this.parameters = parameters;
            this.returnType = returnType;
        }
    }
}