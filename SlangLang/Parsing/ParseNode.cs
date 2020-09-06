using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class ParseNode
    {
        public readonly ParseNodeType nodeType;
        public readonly TextSpan textLocation;

        public ParseNode(ParseNodeType type, TextSpan where)
        {
            nodeType = type;
            textLocation = where;
        } 

        public abstract List<ParseNode> GetChildren();

        public abstract override string ToString();
    }
}