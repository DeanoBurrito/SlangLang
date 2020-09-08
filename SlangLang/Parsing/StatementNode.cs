using System;
using System.Collections.Immutable;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class StatementNode : ParseNode
    {
        public StatementNode(ParseNodeType type, TextSpan where) : base(type, where)
        {}
    }
}