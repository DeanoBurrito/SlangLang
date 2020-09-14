using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public readonly LabelSymbol label;

        public BoundGotoStatement(LabelSymbol symbol, TextSpan where) : base(BoundNodeType.GotoStatement, where)
        {
            label = symbol;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>();
        }

        public override string ToString()
        {
            return "[Goto] " + label;
        }
    }
}