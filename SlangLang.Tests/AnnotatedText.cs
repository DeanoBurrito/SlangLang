using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Xunit;
using SlangLang.Debug;

namespace SlangLang.Tests
{
    internal sealed class AnnotatedText
    {
        public readonly string text;
        public readonly ImmutableArray<TextSpan> spans;
        
        private static string Unindent(string text)
        {
            string[] lines = UnindentLines(text);
            return string.Join(Environment.NewLine, lines);
        }

        public static string[] UnindentLines(string text)
        {
            List<string> lines = new List<string>();
            using (StringReader reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            int minIndentation = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                int identation = line.Length - line.TrimStart().Length;
                minIndentation = Math.Min(minIndentation, identation);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length == 0)
                    continue;
                lines[i] = lines[i].Substring(minIndentation);
            }

            while (lines.Count > 0 && lines[0].Length == 0)
                lines.RemoveAt(0);
            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
                lines.RemoveAt(lines.Count - 1);
            return lines.ToArray();
        }

        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            StringBuilder textBuilder = new StringBuilder();
            List<(int start, int end)> spanRegions = new List<(int start, int end)>();
            Stack<int> startStack = new Stack<int>();
            int pos = 0;

            foreach (char c in text)
            {
                if (c == '[')
                {
                    startStack.Push(pos);
                }
                else if (c == ']')
                {
                    if (startStack.Count == 0)
                    {
                        throw new Exception("Invalid ']' in annotated text, no opening bracket was found!");
                    }
                    
                    int start = startStack.Pop();
                    spanRegions.Add((start, pos));
                }
                else
                {
                    pos++;
                    textBuilder.Append(c);
                }
            }

            if (startStack.Count != 0)
                throw new Exception("Uneven number of bracket pairs in annotated text, stack was not empty at end of parse!");
            
            string[] storeLines = textBuilder.ToString().Split(Environment.NewLine);
            for (int i = 0; i < storeLines.Length; i++)
                storeLines[i] = storeLines[i] + Environment.NewLine;
            TextStore store = new TextStore("Tests", storeLines);
            ImmutableArray<TextSpan>.Builder spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            foreach ((int start, int end) region in spanRegions)
            {
                TextSpan span = new TextSpan(store.GetLocation(region.start), store.GetLocation(region.end));
                spanBuilder.Add(span);
            }

            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            this.text = text;
            this.spans = spans;
        }
    }
}