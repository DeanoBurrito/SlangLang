using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SlangLang.Symbols
{
    internal static class BuildInFunctions
    {
        public static readonly FunctionSymbol Print = new FunctionSymbol("print", ImmutableArray.Create<ParameterSymbol>(
            new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol Input = new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty,
            TypeSymbol.String);
        public static readonly FunctionSymbol Random = new FunctionSymbol("rand", ImmutableArray.Create<ParameterSymbol>(
            new ParameterSymbol("max", TypeSymbol.Int)), TypeSymbol.Void);

        internal static IEnumerable<FunctionSymbol> GetAll()
        {
            return typeof(BuildInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(FunctionSymbol))
                .Select(f => (FunctionSymbol)f.GetValue(null));
        }
    }
}