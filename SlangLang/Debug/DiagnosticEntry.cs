using System;

namespace SlangLang.Debug
{
    public class DiagnosticEntry
    {
        public readonly string module;
        public readonly string message;
        public readonly TextLocation where;
        public DateTime when;

        public DiagnosticEntry(string sender, string msg, TextLocation location, DateTime dt)
        {
            module = sender;
            message = msg;
            where = location;
            when = dt;
        }
    }
}