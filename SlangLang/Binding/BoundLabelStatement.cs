using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public readonly LabelSymbol label;

        public BoundLabelStatement(LabelSymbol symbol, TextSpan where) : base(BoundNodeType.LabelStatement, where)
        {
            label = symbol;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>();
        }

        public override string ToString()
        {
            return "[Label] " + label;
        }
    }
}