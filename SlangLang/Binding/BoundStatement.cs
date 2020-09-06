using System;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal abstract class BoundStatement : BoundNode
    {
        public BoundStatement(BoundNodeType type, TextSpan where) : base(type, where)
        {}
    }
}