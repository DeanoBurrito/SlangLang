using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    public sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> variables = new Dictionary<string, VariableSymbol>();
        public BoundScope parent;

        public BoundScope(BoundScope parentScope)
        {
            parent = parentScope;
        }

        public bool TryLookup(string name, out VariableSymbol variable)
        {
            if (variables.TryGetValue(name, out variable))
                return true;
            if (parent == null)
                return false;
            return parent.TryLookup(name, out variable);
        }

        public bool TryDeclare(VariableSymbol variable)
        {
            if (parent != null && parent.TryLookup(variable.name, out _))
                return false;
            if (variables.ContainsKey(variable.name))
                return false;
            variables.Add(variable.name, variable);
            return true;
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return variables.Values.ToImmutableArray();
        }
    }
}