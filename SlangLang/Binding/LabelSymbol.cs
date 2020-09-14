using System;

namespace SlangLang.Binding
{
    public sealed class LabelSymbol
    {
        public readonly string name;

        internal LabelSymbol(string vName)
        {
            name = vName;
        }

        public override string ToString()
        {
            return name;
        }
    }
}