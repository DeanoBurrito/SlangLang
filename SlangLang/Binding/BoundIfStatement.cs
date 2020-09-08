using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public readonly BoundExpression condition;
        public readonly BoundStatement body;
        public readonly BoundStatement elseStatement;

        public BoundIfStatement(BoundExpression condition, BoundStatement body, BoundStatement elseStatement, TextSpan where) 
            : base(BoundNodeType.IfStatement, where)
        {
            this.condition = condition;
            this.body = body;
            this.elseStatement = elseStatement;
        }

        public override List<BoundNode> GetChildren()
        {
            if (elseStatement == null)
                return new List<BoundNode>() { condition, body };
            else
                return new List<BoundNode>() { condition, body, elseStatement };
        }

        public override string ToString()
        {
            return "[IfStatement]";
        }
    }
}