using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class NameExpression : ExpressionNode
    {   
        public NameExpression(LanguageToken identifier, TextLocation where) : base(identifier, ExpressionNodeType.Name, where)
        {
            
        }

        public override List<ExpressionNode> GetChildren()
        {
            return new List<ExpressionNode>(0);
        }

        public override string ToString() 
        {
            return "[Name] " + base.token.text;
        }
    }
}