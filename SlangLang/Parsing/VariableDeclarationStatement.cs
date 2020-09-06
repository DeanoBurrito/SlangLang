using System;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public sealed class VariableDeclarationStatement : StatementNode
    {
        public readonly LanguageToken keyword;
        public readonly LanguageToken identifier;
        public readonly LanguageToken equals;
        public readonly ExpressionNode intializer;
        public readonly LanguageToken semicolon;
        public readonly bool isReadOnly;
        
        public VariableDeclarationStatement(LanguageToken keyword, LanguageToken identifier, LanguageToken equals, 
            ExpressionNode initializer, LanguageToken semicolon, bool readOnly, TextSpan where) 
            : base(where, ParseNodeType.VariableDeclaration)
        {
            this.keyword = keyword;
            this.identifier = identifier;
            this.equals = equals;
            this.intializer = initializer;
            this.isReadOnly = readOnly;
            this.semicolon = semicolon;
        }
    }
}