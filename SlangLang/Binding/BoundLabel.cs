using System;

namespace SlangLang.Binding
{
    public sealed class BoundLabel
    {
        public readonly string name;

        internal BoundLabel(string vName)
        {
            name = vName;
        }

        public override string ToString()
        {
            return name;
        }
    }
}