using System.Collections.Generic;
using System.Linq;
using SlangLang.Debug;

namespace SlangLang.Parsing
{   
    public sealed class CallExpression : ExpressionNode
    {
        public readonly LanguageToken openParanthesis;
        public readonly LanguageToken closeParanthesis;
        public readonly SeparatedNodeList<ExpressionNode> arguments;
        
        public CallExpression(LanguageToken identifier, LanguageToken openParanthesis, SeparatedNodeList<ExpressionNode> arguments, LanguageToken closeParanthesis)
            : base(identifier, ParseNodeType.CallExpression, identifier.textLocation)
        {
            this.openParanthesis = openParanthesis;
            this.closeParanthesis = closeParanthesis;
            this.arguments = arguments;
        }

        public override List<ParseNode> GetChildren()
        {
            List<ParseNode> children = new List<ParseNode>();
            for (int i = 0; i < arguments.count; i++)
                children.Add(arguments[i]);
            return children;
        }

        public override string ToString()
        {
            return "[CallExpr] " + base.ToString();
        }
    }
}