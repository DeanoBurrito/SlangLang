using System;
using System.Reflection;

namespace SlangLang
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReplMetaCmdAttribute : Attribute
    {
        public readonly string identifier;
        public readonly string helptext;

        public ReplMetaCmdAttribute(string ident, string help)
        {
            identifier = ident;
            helptext = help;
        }
    }
}