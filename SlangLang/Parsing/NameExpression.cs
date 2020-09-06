using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class NameExpression : ExpressionNode
    {   
        public NameExpression(LanguageToken identifier, TextSpan where) : base(identifier, ParseNodeType.Name, where)
        {
            
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>();
        }

        public override string ToString() 
        {
            return "[Name] " + base.token.text;
        }
    }
}