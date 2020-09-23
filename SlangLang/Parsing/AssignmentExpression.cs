using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class AssignmentExpression : ExpressionNode
    {   
        public readonly ExpressionNode expression;
        
        public AssignmentExpression(LanguageToken identifier, ExpressionNode expression) 
            : base(identifier, ParseNodeType.AssignmentExpression, new TextSpan(identifier.textLocation.start, expression.textLocation.end))
        {
            this.expression = expression;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { expression };
        }

        public override string ToString() 
        {
            return "[Assignment] => " + base.token.value;
        }
    }
}