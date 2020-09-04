using System;
using System.Collections.Immutable;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundGlobalScope
    {
        public readonly BoundGlobalScope previous;
        public readonly Diagnostics diagnostics;
        public readonly ImmutableArray<VariableSymbol> variables;
        public readonly BoundExpression expression;
        
        public BoundGlobalScope(BoundGlobalScope prev, Diagnostics diags, ImmutableArray<VariableSymbol> vars, BoundExpression expr)
        {
            previous = prev;
            diagnostics = diags;
            variables = vars;
            expression = expr;
        }
    }
}