using System;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class BoundBinaryOperator
    {
        public readonly LanguageTokenType langToken;
        public readonly BoundBinaryOperatorType binaryOperator;
        public readonly Type leftOperandType;
        public readonly Type rightOperandType;
        public readonly Type resultType;
        
        private BoundBinaryOperator(LanguageTokenType token, BoundBinaryOperatorType binaryOp, Type leftOpType, Type rightOpType, Type resultType)
        {
            langToken = token;
            binaryOperator = binaryOp;
            leftOperandType = leftOpType;
            rightOperandType = rightOpType;
            this.resultType = resultType;
        }

        private BoundBinaryOperator(LanguageTokenType token, BoundBinaryOperatorType binaryOp, Type type) : this(token, binaryOp, type, type, type)
        {}

        public override string ToString()
        {
            return binaryOperator + " (" + leftOperandType + ", " + rightOperandType + ") => " + resultType;
        }

        private static BoundBinaryOperator[] ops = 
        {
            new BoundBinaryOperator(LanguageTokenType.Plus, BoundBinaryOperatorType.Addition, typeof(int)),
            new BoundBinaryOperator(LanguageTokenType.Minus, BoundBinaryOperatorType.Subtract, typeof(int)),
            new BoundBinaryOperator(LanguageTokenType.Star, BoundBinaryOperatorType.Multiplication, typeof(int)),
            new BoundBinaryOperator(LanguageTokenType.ForwardSlash, BoundBinaryOperatorType.Division, typeof(int)),

            new BoundBinaryOperator(LanguageTokenType.PipePipe, BoundBinaryOperatorType.ConditionalOr, typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.AndAnd, BoundBinaryOperatorType.ConditionalAnd, typeof(bool)),
        };

        public static BoundBinaryOperator Bind(LanguageTokenType langTokenType, Type leftType, Type rightType)
        {
            foreach (BoundBinaryOperator op in ops)
            {
                if (op.langToken == langTokenType && op.leftOperandType == leftType && op.rightOperandType == rightType)
                    return op;
            }
            return null;
        }
    }
}