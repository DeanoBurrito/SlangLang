using System;
using System.Collections.Immutable;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class StatementNode : ParseNode
    {
        public StatementNode(TextSpan where, ParseNodeType type) : base(type, where)
        {}
    }
}