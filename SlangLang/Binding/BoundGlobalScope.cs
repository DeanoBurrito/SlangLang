using System;
using System.Collections.Immutable;
using SlangLang.Debug;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class BoundGlobalScope
    {
        public readonly BoundGlobalScope previous;
        public readonly Diagnostics diagnostics;
        public readonly ImmutableArray<VariableSymbol> variables;
        public readonly BoundStatement statement;
        
        public BoundGlobalScope(BoundGlobalScope prev, Diagnostics diags, ImmutableArray<VariableSymbol> vars, BoundStatement statement)
        {
            previous = prev;
            diagnostics = diags;
            variables = vars;
            this.statement = statement;
        }
    }
}