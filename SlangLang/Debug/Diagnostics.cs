using System;
using System.Collections.Generic;

namespace SlangLang.Debug
{
    public sealed class Diagnostics
    {
        public static Diagnostics DummyInstance = new Diagnostics();
        
        Dictionary<string, List<DiagnosticEntry>> infoEntries;
        Dictionary<string, List<DiagnosticEntry>> warningEntries;
        Dictionary<string, List<DiagnosticEntry>> failureEntries;
        (ConsoleColor fg, ConsoleColor bg) infoColor = (ConsoleColor.White, ConsoleColor.Black);
        (ConsoleColor fg, ConsoleColor bg) warningColor = (ConsoleColor.Yellow, ConsoleColor.Black);
        (ConsoleColor fg, ConsoleColor bg) failureColor = (ConsoleColor.Red, ConsoleColor.Black);

        DateTime initTime;
        bool dummyInstance;
        
        public bool HasErrors { get; private set; }

        internal Diagnostics()
        {
            dummyInstance = true;
        }
        
        public Diagnostics(DateTime startTime)
        { 
            initTime = startTime;
            dummyInstance = false;

            infoEntries = new Dictionary<string, List<DiagnosticEntry>>();
            warningEntries = new Dictionary<string, List<DiagnosticEntry>>();
            failureEntries = new Dictionary<string, List<DiagnosticEntry>>();
        }

        public void Aggregate(Diagnostics diagnostics)
        {
            if (diagnostics.dummyInstance || dummyInstance)
                return;
            
            if (diagnostics.initTime < initTime)
                initTime = diagnostics.initTime;
            HasErrors = diagnostics.HasErrors || HasErrors;
            
            foreach (KeyValuePair<string, List<DiagnosticEntry>> pair in diagnostics.infoEntries)
            {
                if (!infoEntries.ContainsKey(pair.Key))
                    infoEntries.Add(pair.Key, new List<DiagnosticEntry>());
                infoEntries[pair.Key].AddRange(pair.Value);
            }
            foreach (KeyValuePair<string, List<DiagnosticEntry>> pair in diagnostics.warningEntries)
            {
                if (!warningEntries.ContainsKey(pair.Key))
                    warningEntries.Add(pair.Key, new List<DiagnosticEntry>());
                warningEntries[pair.Key].AddRange(pair.Value);
            }
            foreach (KeyValuePair<string, List<DiagnosticEntry>> pair in diagnostics.failureEntries)
            {
                if (!failureEntries.ContainsKey(pair.Key))
                    failureEntries.Add(pair.Key, new List<DiagnosticEntry>());
                failureEntries[pair.Key].AddRange(pair.Value);
            }
        }

        public void AddInfo(string module, string message, DateTime when)
        {
            if (dummyInstance)
                return;

            if (!infoEntries.ContainsKey(module))
                infoEntries.Add(module, new List<DiagnosticEntry>());
            infoEntries[module].Add(new DiagnosticEntry(module, message, null, when));
        }

        public void AddWarning(string module, string message, TextLocation where, DateTime when)
        {
            if (dummyInstance)
                return;

            if (!warningEntries.ContainsKey(module))
                warningEntries.Add(module, new List<DiagnosticEntry>());
            warningEntries[module].Add(new DiagnosticEntry(module, message, where, when));
        }

        public void AddFailure(string module, string message, TextLocation where, DateTime when)
        {
            if (dummyInstance)
                return;

            HasErrors = true;
            if (!failureEntries.ContainsKey(module))
                failureEntries.Add(module, new List<DiagnosticEntry>());
            failureEntries[module].Add(new DiagnosticEntry(module, message, where, when));
        }

        public void Clear()
        {
            if (dummyInstance)
                return;

            HasErrors = false;
            infoEntries.Clear();
            warningEntries.Clear();
            failureEntries.Clear();
        }

        public void WriteToStandardOut()
        {   
            if (dummyInstance)
                return;

            const int wrapIndent = 8;
            int terminalWidth = Console.WindowWidth - wrapIndent;
            
            Console.WriteLine("[SlangLang Diagnostics " + DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString() + "]");
            Console.WriteLine("Run initialized at " + initTime.ToString("HH:mm:ss.fffff") + ", " + (DateTime.Now - initTime).TotalSeconds.ToString("###,##0.00000") + " seconds ago.");

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

            if (failureEntries.Count == 0)
                Console.WriteLine("No errors. Process complete.");

        }
    }
}