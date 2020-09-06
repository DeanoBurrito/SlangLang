using System;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal sealed class BoundVariableDeclaration : BoundStatement
    {
        public readonly VariableSymbol variable;
        public readonly BoundExpression initializer;

        public BoundVariableDeclaration(VariableSymbol variable, BoundExpression initializer, TextSpan where) : base(BoundNodeType.VariableDeclarationStatement, where)
        {
            this.initializer = initializer;
            this.variable = variable;
        }
    }
}