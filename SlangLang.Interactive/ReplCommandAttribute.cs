using System;
using System.Reflection;

namespace SlangLang.Interactive
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReplCommandAttribute : Attribute
    {
        public readonly string identifier;
        public readonly string helptext;

        public ReplCommandAttribute(string ident, string help)
        {
            identifier = ident;
            helptext = help;
        }
    }
}