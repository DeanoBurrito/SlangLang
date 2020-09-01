using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class NameExpression : ExpressionNode
    {
        public readonly LanguageToken identifierToken;
        
        public NameExpression(LanguageToken identifier, TextLocation where) : base(ExpressionNodeType.Name, where)
        {
            identifierToken = identifier;
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>(0);
        }

        public override string ToString() 
        {
            return "[Name] " + identifierToken.text;
        }
    }
}