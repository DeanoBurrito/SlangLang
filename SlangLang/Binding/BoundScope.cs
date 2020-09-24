using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    public sealed class BoundScope
    {
        private Dictionary<string, Symbol> symbols;
        public BoundScope parent;

        public BoundScope(BoundScope parentScope)
        {
            parent = parentScope;
        }

        public bool TryLookupVariable(string name, out VariableSymbol symbol)
            => TryLookupSymbol(name, out symbol);

        public bool TryDeclareVariable(VariableSymbol symbol)
            => TryDeclareSymbol(symbol);

        public bool TryLookupFunction(string name, out FunctionSymbol symbol)
            => TryLookupSymbol(name, out symbol);

        public bool TryDeclareFunction(FunctionSymbol symbol)
            => TryDeclareSymbol(symbol);

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
            => GetDeclaredSymbols<VariableSymbol>();
        
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
            => GetDeclaredSymbols<FunctionSymbol>();

        private bool TryLookupSymbol<T>(string name, out T symbol)
            where T : Symbol
        {
            symbol = null;
            if (symbols != null && symbols.TryGetValue(name, out Symbol declared))
            {
                if (declared is T matchingSymbol)
                {
                    symbol = matchingSymbol;
                    return true;
                }
                return false;
            }

            if (parent == null)
                return false;

            return parent.TryLookupSymbol<T>(name, out symbol);
        }

        private bool TryDeclareSymbol<T>(T symbol)
            where T : Symbol
        {
            if (symbols == null)
                symbols = new Dictionary<string, Symbol>();

            if (parent != null && parent.TryLookupSymbol<T>(symbol.name, out _))
                return false; //prevent shadowing (reassigning of symbols in narrower scopes)
            if (symbols.ContainsKey(symbol.name))
                return false;

            symbols.Add(symbol.name, symbol);
            return true;
        }

        private ImmutableArray<T> GetDeclaredSymbols<T>()
            where T : Symbol
        {
            if (symbols == null)
                return ImmutableArray<T>.Empty;
            return symbols.Values.OfType<T>().ToImmutableArray();
        }
    }
}