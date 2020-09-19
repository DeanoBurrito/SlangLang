using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SlangLang.Tests")]
namespace SlangLang.Symbols
{    
    public sealed class VariableSymbol : Symbol
    {
        public readonly TypeSymbol type;
        public readonly bool isReadOnly;

        internal VariableSymbol(string vName, bool isReadonly, TypeSymbol vType) : base(SymbolType.Variable, vName)
        {
            type = vType;
            isReadOnly = isReadonly;
        }

        public override string ToString()
        {
            return base.ToString() + " (" + type + ") " + (isReadOnly ? "ReadOnly" : "");
        }
    }
}