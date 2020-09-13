using System;
using SlangLang.Binding;

namespace SlangLang.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        public static BoundStatement Lower(BoundStatement statement)
        {
            Lowerer lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);
        }
        
        private Lowerer()
        {

        }
    }
}