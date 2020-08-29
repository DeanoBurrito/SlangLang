using System;

namespace SlangLang.Debug
{
    public sealed class TextLocation
    {
        public static TextLocation NoLocation = new TextLocation("No Source Location", 0, 0);

        public string filename;
        public int line;
        public int column;
        public int length;

        public TextLocation(string fname, int line, int column)
        {
            filename = fname;
            this.line = line;
            this.column = column;
            length = 1;
        }

        public override string ToString()
        {
            if (filename == "No Source Location")
                return filename;
            return "[" + filename + ": line " + line + ", col " + column + (length != 0 ? ", len " + length : "") + "]";
        }
    }
}