using System;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class BoundUnaryOperator
    {
        public readonly LanguageTokenType opToken;
        public readonly BoundUnaryOperatorType unaryOperator;
        public readonly Type operandType;
        public readonly Type resultType;
        
        private BoundUnaryOperator(LanguageTokenType opToken, BoundUnaryOperatorType unaryOp, Type operandType, Type resultType)
        {
            this.opToken = opToken;
            unaryOperator = unaryOp;
            this.operandType = operandType;
            this.resultType = resultType;
        }

        private BoundUnaryOperator(LanguageTokenType opToken, BoundUnaryOperatorType unaryOp, Type operandType) : this(opToken, unaryOp, operandType, operandType)
        {}

        public override string ToString()
        {
            return unaryOperator + " (" + operandType + ") => " + resultType;
        }

        private static BoundUnaryOperator[] ops = 
        {
            new BoundUnaryOperator(LanguageTokenType.Minus, BoundUnaryOperatorType.Negate, typeof(int)),
            
            new BoundUnaryOperator(LanguageTokenType.Exclamation, BoundUnaryOperatorType.Not, typeof(bool)),
        };

        public static BoundUnaryOperator Bind(LanguageTokenType langTokenType, Type operandType)
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