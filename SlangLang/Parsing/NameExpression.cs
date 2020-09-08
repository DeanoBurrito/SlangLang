using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class NameExpression : ExpressionNode
    {   
        public NameExpression(LanguageToken identifier) 
            : base(identifier, ParseNodeType.Name, identifier.textLocation)
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