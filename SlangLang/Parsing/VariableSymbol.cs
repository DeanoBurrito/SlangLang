using System;

namespace SlangLang.Parsing
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