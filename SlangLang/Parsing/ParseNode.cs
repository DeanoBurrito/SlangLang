using System;
using System.Collections.Generic;
using System.Reflection;
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
    }
}