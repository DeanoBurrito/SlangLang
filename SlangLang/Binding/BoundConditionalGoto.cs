using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundConditionalGoto : BoundStatement
    {
        public readonly LabelSymbol label;
        public readonly BoundExpression condition;
        public readonly bool jumpIfTrue;

        public BoundConditionalGoto(LabelSymbol symbol, BoundExpression condition, TextSpan where, bool jumpIfTrue = true) : base(BoundNodeType.ConditionalGotoStatement, where)
        {
            label = symbol;
            this.condition = condition;
            this.jumpIfTrue = jumpIfTrue;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() { condition };
        }

        public override string ToString()
        {
            return "[ConditionalGoto] " + label + ", Jump when [" + !jumpIfTrue + "]";
        }
    }
}