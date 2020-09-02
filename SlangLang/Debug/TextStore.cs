using System;

namespace SlangLang.Debug
{
    public sealed class TextStore
    {
        readonly string[] lines;
        readonly int[] lineLengths;
        readonly int length;
        readonly string filename;
        
        public TextStore(string fname, string[] fileLines)
        {
            filename = fname;
            lines = fileLines;
            lineLengths = new int[lines.Length];
            length = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                lineLengths[i] = lines[i].Length;
                length += lines[i].Length;
            }
        }

        public int GetLength()
        {
            return length;
        }

        public char GetCharAt(int index)
        {
            int line = FindLine(index, out int linePos);
            if (line == -1)
                return '\0';
            return lines[line][linePos];
        }

        public string GetSubstring(int start, int len)
        {
            int lineNum = FindLine(start, out int hPos);
            if (lineNum == -1)
                return "";
            string substr = lines[lineNum].Remove(0, hPos);
            while (substr.Length < len)
            {
                if (lineNum >= lines.Length)
                    return substr;
                lineNum++;
                substr += lines[lineNum];
            }
            if (substr.Length > len)
                substr = substr.Remove(len);
            return substr;
        }

        public string GetSubstring(TextSpan span)
        {
            if (span.start.absoluteFilePosition >= 0 && span.length > 0)
                return GetSubstring(span.start.absoluteFilePosition, span.length);
            return "";
        }

        public TextLocation GetLocation(int index)
        {
            int lineNum = FindLine(index, out int col);
            if (lineNum == -1)
                return TextLocation.NoLocation;
            return new TextLocation(filename, lineNum, col, index);
        }

        private int FindLine(int index, out int idxRemainder)
        {
            idxRemainder = index;
            int curLineLen = lineLengths[0];
            int curLine = 0;
            while (index >= curLineLen)
            {
                curLine++;
                if (curLine >= lineLengths.Length)
                    return -1; //means it dosnt exist, return usable error in upstream code
                index -= curLineLen;
                curLineLen = lineLengths[curLine];
            }
            idxRemainder = index;
            return curLine;
        }
    }
}