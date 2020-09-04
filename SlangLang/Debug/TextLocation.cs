using System;

namespace SlangLang.Debug
{
    public sealed class TextLocation
    {
        public static TextLocation NoLocation = new TextLocation("No Source Location", 0, 0);

        public readonly string filename;
        public readonly int line;
        public readonly int column;
        internal readonly int absoluteFilePosition = -1;

        public TextLocation(string fname, int line, int column)
        {
            filename = fname;
            this.line = line;
            this.column = column;
        }

        internal TextLocation(string fname, int line, int column, int absPosition) : this(fname, line, column)
        {
            absoluteFilePosition = absPosition;
        }

        public TextLocation(TextLocation copy)
        {
            filename = copy.filename;
            line = copy.line;
            column = copy.column;
        }

        public override string ToString()
        {
            if (filename == "No Source Location")
                return filename;
            return "[" + filename + ": line " + (line + 1) + ", col " + (column + 1) + "]";
        }
    }
}