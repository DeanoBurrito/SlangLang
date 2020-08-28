using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SlangLang.Debugging
{
    public class Diagnostics
    {
        //Dictionary<string, DiagnosticEntry> entries = new Dictionary<string, DiagnosticEntry>();
        Dictionary<string, List<DiagnosticEntry>> infoEntries = new Dictionary<string, List<DiagnosticEntry>>();
        Dictionary<string, List<DiagnosticEntry>> warningEntries = new Dictionary<string, List<DiagnosticEntry>>();
        Dictionary<string, List<DiagnosticEntry>> failureEntries = new Dictionary<string, List<DiagnosticEntry>>();
        (ConsoleColor fg, ConsoleColor bg) infoColor = (ConsoleColor.White, ConsoleColor.Black);
        (ConsoleColor fg, ConsoleColor bg) warningColor = (ConsoleColor.Yellow, ConsoleColor.Black);
        (ConsoleColor fg, ConsoleColor bg) failureColor = (ConsoleColor.Red, ConsoleColor.Black);

        
        public bool HasErrors { get; private set; }
        
        public Diagnostics()
        {}

        public void AddInfo(string module, string message, DateTime when)
        {
            if (!infoEntries.ContainsKey(module))
                infoEntries.Add(module, new List<DiagnosticEntry>());
            infoEntries[module].Add(new DiagnosticEntry(module, message, null, when));
        }

        public void AddWarning(string module, string message, TextLocation where, DateTime when)
        {
            if (!warningEntries.ContainsKey(module))
                warningEntries.Add(module, new List<DiagnosticEntry>());
            warningEntries[module].Add(new DiagnosticEntry(module, message, where, when));
        }

        public void AddFailure(string module, string message, TextLocation where, DateTime when)
        {
            HasErrors = true;
            if (!failureEntries.ContainsKey(module))
                failureEntries.Add(module, new List<DiagnosticEntry>());
            failureEntries[module].Add(new DiagnosticEntry(module, message, where, when));
        }

        public void Clear()
        {
            HasErrors = false;
            infoEntries.Clear();
            warningEntries.Clear();
            failureEntries.Clear();
        }

        public void WriteToStandardOut()
        {
            const int wrapIndent = 8;
            int terminalWidth = Console.WindowWidth - wrapIndent;
            
            Console.WriteLine("[SlangLang Diagnostics " + DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString() + "]");

            Console.ForegroundColor = infoColor.fg;
            Console.BackgroundColor = infoColor.bg;
            foreach (KeyValuePair<string, List<DiagnosticEntry>> moduleEntries in infoEntries)
            {
                Console.WriteLine("[" + moduleEntries.Key + " - Info]");
                foreach (DiagnosticEntry entry in moduleEntries.Value)
                {
                    string remainingStr = entry.when.ToString("HH:mm:ss.fffff") + " - " + entry.message;
                    bool prependIndent = false;
                    while (remainingStr.Length > terminalWidth)
                    {
                        Console.WriteLine((prependIndent ? new string(' ', wrapIndent) : "") + remainingStr.Remove(terminalWidth));
                        remainingStr = remainingStr.Remove(0, terminalWidth);
                        prependIndent = true;
                    }
                    Console.WriteLine((prependIndent ? new string(' ', wrapIndent) : "") + remainingStr);
                }

            }
            Console.WriteLine();

            Console.ForegroundColor = warningColor.fg;
            Console.BackgroundColor = warningColor.bg;
            foreach (KeyValuePair<string, List<DiagnosticEntry>> moduleEntries in warningEntries)
            {
                Console.WriteLine("[" + moduleEntries.Key + " - " + moduleEntries.Value.Count + " warnings from this module.]");
                foreach (DiagnosticEntry entry in moduleEntries.Value)
                {
                    string remainingStr = entry.when.ToString("HH:mm:ss.fffff") + " " + entry.where.ToString() + " - " + entry.message;
                    bool prependIndent = false;
                    while (remainingStr.Length > terminalWidth)
                    {
                        Console.WriteLine((prependIndent ? new string(' ', wrapIndent) : "") + remainingStr.Remove(terminalWidth));
                        remainingStr = remainingStr.Remove(0, terminalWidth);
                        prependIndent = true;
                    }
                    Console.WriteLine((prependIndent ? new string(' ', wrapIndent) : "") + remainingStr);
                }
            }

            Console.ForegroundColor = failureColor.fg;
            Console.BackgroundColor = failureColor.bg;
            foreach (KeyValuePair<string, List<DiagnosticEntry>> moduleEntries in failureEntries)
            {
                Console.WriteLine("[" + moduleEntries.Key + " - " + moduleEntries.Value.Count + " errors from this module.]" );
                foreach (DiagnosticEntry entry in moduleEntries.Value)
                {
                    string remainingStr = entry.when.ToString("HH:mm:ss.fffff") + " " + entry.where.ToString() + " - " + entry.message;
                    bool prependIndent = false;
                    while (remainingStr.Length > terminalWidth)
                    {
                        Console.WriteLine((prependIndent ? new string(' ', wrapIndent) : "") + remainingStr.Remove(terminalWidth));
                        remainingStr = remainingStr.Remove(0, terminalWidth);
                        prependIndent = true;
                    }
                    Console.WriteLine((prependIndent ? new string(' ', wrapIndent) : "") + remainingStr);
                }
            }

            Console.ResetColor();
        }
    }

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