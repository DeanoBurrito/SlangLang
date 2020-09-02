using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class NameExpression : ExpressionNode
    {   
        public NameExpression(LanguageToken identifier, TextSpan where) : base(identifier, ExpressionNodeType.Name, where)
        {
            
        }

        public override string ToString() 
        {
            return "[Name] " + base.token.text;
        }
    }
}