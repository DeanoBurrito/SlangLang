using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SlangLang.Tests")]
namespace SlangLang.Binding
{
    public sealed class VariableSymbol
    {
        public readonly string name;
        public readonly Type type;

        internal VariableSymbol(string vName, Type vType)
        {
            name = vName;
            type = vType;
        }
    }
}