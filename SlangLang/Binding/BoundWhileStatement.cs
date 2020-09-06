using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public readonly BoundExpression condition;
        public readonly BoundStatement body;
        
        public BoundWhileStatement(BoundExpression condition, BoundStatement body, TextSpan where) : base(BoundNodeType.WhileStatement, where)
        {
            this.condition = condition;
            this.body = body;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() { condition, body };
        }

        public override string ToString()
        {
            return "[WhileStatement]";
        }
    }
}