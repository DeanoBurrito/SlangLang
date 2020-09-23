using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    public sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> variables;
        private Dictionary<string, FunctionSymbol> functions;
        public BoundScope parent;

        public BoundScope(BoundScope parentScope)
        {
            parent = parentScope;
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
            variable = null;
            if (variables != null && variables.TryGetValue(name, out variable))
                return true;
            if (parent == null)
                return false;
            return parent.TryLookupVariable(name, out variable);
        }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if (variables == null)
                variables = new Dictionary<string, VariableSymbol>();
            
            if (parent != null && parent.TryLookupVariable(variable.name, out _))
                return false;
            if (variables.ContainsKey(variable.name))
                return false;
            variables.Add(variable.name, variable);
            return true;
        }

        public bool TryLookupFunction(string name, out FunctionSymbol function)
        {
            function = null;
            if (functions != null && functions.TryGetValue(name, out function))
                return true;
            if (parent == null)
                return false;
            return parent.TryLookupFunction(name, out function);
        }

        public bool TryDeclareFunction(FunctionSymbol function)
        {
            if (functions == null)
                functions = new Dictionary<string, FunctionSymbol>();
            
            if (parent != null && parent.TryLookupVariable(function.name, out _))
                return false;
            if (functions.ContainsKey(function.name))
                return false;
            functions.Add(function.name, function);
            return true;
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            if (variables == null)
                return ImmutableArray<VariableSymbol>.Empty;
            return variables.Values.ToImmutableArray();
        }

        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            if (functions == null)
                return ImmutableArray<FunctionSymbol>.Empty;
            return functions.Values.ToImmutableArray();
        }
    }
}