using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public readonly BoundStatement setupStatement;
        public readonly BoundExpression condition;
        public readonly BoundStatement postStatement;
        public readonly BoundStatement body;
        
        public BoundForStatement(BoundStatement setup, BoundExpression condition, BoundStatement post, BoundStatement body, TextSpan where) 
            : base(BoundNodeType.ForStatement, where)
        {
            setupStatement = setup;
            this.condition = condition;
            postStatement = post;
            this.body = body;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>() { setupStatement, condition, postStatement, body };
        }

        public override string ToString()
        {
            return "[ForStatement]";
        }
    }
}