using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundConditionalGoto : BoundStatement
    {
        public readonly LabelSymbol label;
        public readonly BoundExpression condition;
        public readonly bool jumpIfFalse;

        public BoundConditionalGoto(LabelSymbol symbol, BoundExpression condition, TextSpan where, bool jumpIfFalse = false) : base(BoundNodeType.ConditionalGotoStatement, where)
        {
            label = symbol;
            this.condition = condition;
            this.jumpIfFalse = jumpIfFalse;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() { condition };
        }

        public override string ToString()
        {
            return "[ConditionalGoto] " + label + ", Jump when [" + !jumpIfFalse + "]";
        }
    }
}