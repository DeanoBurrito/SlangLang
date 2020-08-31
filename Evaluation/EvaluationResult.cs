using System;
using SlangLang.Debug;

namespace SlangLang.Evaluation
{
    public sealed class EvaluationResult
    {
        public readonly object value;
        public readonly Diagnostics diagnostics;

        internal EvaluationResult(object val, Diagnostics diags)
        {
            value = val;
            diagnostics = diags;
        }
    }
}