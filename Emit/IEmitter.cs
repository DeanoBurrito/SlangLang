using System;
using SlangLang.Expressions;
using SlangLang.Debugging;

namespace SlangLang.Emit
{
    public interface IEmitter
    {
        void FullWrite(object emitConfiguration, Diagnostics diagnostics, ExpressionNode exprTree);
        bool VerifyWrite(object emitConfiguration, Diagnostics diagnostics);
    }
}