using System;
using SlangLang.Parsing;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class BoundUnaryOperator
    {
        public readonly LanguageTokenType opToken;
        public readonly BoundUnaryOperatorType unaryOperator;
        public readonly TypeSymbol operandType;
        public readonly TypeSymbol resultType;
        
        private BoundUnaryOperator(LanguageTokenType opToken, BoundUnaryOperatorType unaryOp, TypeSymbol operandType, TypeSymbol resultType)
        {
            this.opToken = opToken;
            unaryOperator = unaryOp;
            this.operandType = operandType;
            this.resultType = resultType;
        }

        private BoundUnaryOperator(LanguageTokenType opToken, BoundUnaryOperatorType unaryOp, TypeSymbol operandType) : this(opToken, unaryOp, operandType, operandType)
        {}

        public override string ToString()
        {
            return unaryOperator + " (" + operandType + ") => " + resultType;
        }

        private static BoundUnaryOperator[] ops = 
        {
            new BoundUnaryOperator(LanguageTokenType.Minus, BoundUnaryOperatorType.Negate, TypeSymbol.Int),
            
            new BoundUnaryOperator(LanguageTokenType.Exclamation, BoundUnaryOperatorType.Not, TypeSymbol.Bool),
        };

        public static BoundUnaryOperator Bind(LanguageTokenType langTokenType, TypeSymbol operandType)
        {
            foreach (BoundUnaryOperator op in ops)
            {
                if (op.opToken == langTokenType && op.operandType == operandType)
                    return op;
            }
            return null;
        }
    }
}