using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class AssignmentExpression : ExpressionNode
    {   
        public readonly LanguageToken identiferToken;
        public readonly ExpressionNode expression;
        
        public AssignmentExpression(LanguageToken identifier, ExpressionNode expression, TextLocation where) : base(ExpressionNodeType.Assignment, where)
        {
            identiferToken = identifier;
            this.expression = expression;
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>() { expression };
        }

        public override string ToString() 
        {
            return "[Assignment] => " + identiferToken.text;
        }
    }
}