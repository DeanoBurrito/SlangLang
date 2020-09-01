using System;

namespace SlangLang.Debug
{
    public sealed class TextStore
    {
        readonly string[] lines;
        readonly int[] lineLengths;
        readonly int length;
        
        public TextStore(string[] fileLines)
        {
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

        public TextLocation GetLocation(int index)
        {
            int lineNum = FindLine(index, out int col);
            if (lineNum == -1)
                return TextLocation.NoLocation;
            return new TextLocation("", lineNum, col);
        }

        private int FindLine(int index, out int idxRemainder)
        {
            int idx = index;
            idxRemainder = idx;
            int curLineLen = lineLengths[0];
            int curLine = 0;
            while (idx > curLineLen)
            {
                curLine++;
                if (curLine >= lineLengths.Length)
                    return -1; //means it dosnt exist, return usable error in upstream code
                idx -= curLineLen;
                curLineLen = lineLengths[curLine];
            }
            idxRemainder = idx;
            return curLine;
        }
    }
}