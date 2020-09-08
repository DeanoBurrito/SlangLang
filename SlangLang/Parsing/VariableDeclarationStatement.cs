using System;
using System.Collections.Generic;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class VariableDeclarationStatement : StatementNode
    {
        public readonly LanguageToken keyword;
        public readonly LanguageToken identifier;
        public readonly LanguageToken equals;
        public readonly ExpressionNode initializer;
        public readonly LanguageToken semicolon;
        public readonly bool isReadOnly;
        
        public VariableDeclarationStatement(LanguageToken keyword, LanguageToken identifier, LanguageToken equals, 
            ExpressionNode initializer, LanguageToken semicolon, bool readOnly) 
            : base(ParseNodeType.VariableDeclaration, new TextSpan(keyword.textLocation.start, semicolon.textLocation.end))
        {
            this.keyword = keyword;
            this.identifier = identifier;
            this.equals = equals;
            this.initializer = initializer;
            this.isReadOnly = readOnly;
            this.semicolon = semicolon;
        }

        public override List<ParseNode> GetChildren()
        {
            return new List<ParseNode>() { initializer };
        }

        public override string ToString()
        {
            return "[VariableDeclaration]" + keyword.text + " " + identifier.text;
        }
    }
}