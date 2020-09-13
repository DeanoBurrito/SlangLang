using System;

namespace SlangLang.Debug
{
    public sealed class TextSpan
    {
        public static TextSpan NoText = new TextSpan();

        public readonly TextLocation start;
        public readonly TextLocation end;
        public readonly int length;

        private TextSpan()
        {
            start = TextLocation.NoLocation;
            end = TextLocation.NoLocation;
            length = 0;
        }

        public TextSpan(TextLocation begin)
        {
            start = begin;
            end = new TextLocation(begin);
            length = 1;
        }

        public TextSpan(TextLocation begin, TextLocation end, int charLength)
        {
            start = begin;
            this.end = end;
            length = charLength;
        }

        public TextSpan(TextLocation begin, TextLocation end)
        {
            start = begin;
            this.end = end;
            length = begin.filename == end.filename ? end.absoluteFilePosition - begin.absoluteFilePosition : 0;
        }

        public override string ToString()
        {
            return start.ToString().Replace("]", "") + ", " + end.ToString().Replace("[", "");
        }

        public override bool Equals(object obj)
        {
            if (obj is TextSpan span)
            {
                if (span.start.absoluteFilePosition == this.start.absoluteFilePosition &&
                    span.end.absoluteFilePosition == this.end.absoluteFilePosition &&
                    span.length == this.length)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}