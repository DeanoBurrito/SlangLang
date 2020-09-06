using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SlangLang.Tests")]
namespace SlangLang.Binding
{
    public sealed class VariableSymbol
    {
        public readonly string name;
        public readonly Type type;
        public readonly bool isReadOnly;

        internal VariableSymbol(string vName, bool isReadonly, Type vType)
        {
            name = vName;
            type = vType;
            isReadOnly = isReadonly;
        }
    }
}