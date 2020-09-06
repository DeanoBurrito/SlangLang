using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class AssignmentExpression : ExpressionNode
    {   
        public readonly ExpressionNode expression;
        
        public AssignmentExpression(LanguageToken identifier, ExpressionNode expression, TextSpan where) : base(identifier, ParseNodeType.Assignment, where)
        {
            this.expression = expression;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { expression };
        }

        public override string ToString() 
        {
            return "[Assignment] => " + base.token.text;
        }
    }
}