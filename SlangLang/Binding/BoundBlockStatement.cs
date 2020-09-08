using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public readonly ImmutableArray<BoundStatement> statements;
        
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements, TextSpan where) 
            : base (BoundNodeType.BlockStatement, where)
        {
            this.statements = statements;
        }

        public override List<BoundNode> GetChildren()
        {
            return new List<BoundNode>(statements);
        }

        public override string ToString()
        {
            return "[BlockStatement]";
        }
    }
}