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

        private BoundBinaryOperator(LanguageTokenType tokenType, BoundBinaryOperatorType binaryOp, Type type, Type resultType) : this(tokenType, binaryOp, type, type, resultType)
        {}

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
            new BoundBinaryOperator(LanguageTokenType.Pipe, BoundBinaryOperatorType.BitwiseOr, typeof(int)),
            new BoundBinaryOperator(LanguageTokenType.And, BoundBinaryOperatorType.BitwiseAnd, typeof(int)),
            new BoundBinaryOperator(LanguageTokenType.Circumflex, BoundBinaryOperatorType.BitwiseXor, typeof(int)),

            new BoundBinaryOperator(LanguageTokenType.EqualsEquals, BoundBinaryOperatorType.Equals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.ExclamationEquals, BoundBinaryOperatorType.NotEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.Less, BoundBinaryOperatorType.LessThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.LessOrEquals, BoundBinaryOperatorType.LessThanOrEqual, typeof(int), typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.Greater, BoundBinaryOperatorType.GreaterThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.GreaterOrEquals, BoundBinaryOperatorType.GreaterThanOrEqual, typeof(int), typeof(bool)),

            new BoundBinaryOperator(LanguageTokenType.Pipe, BoundBinaryOperatorType.BitwiseOr, typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.PipePipe, BoundBinaryOperatorType.ConditionalOr, typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.And, BoundBinaryOperatorType.BitwiseAnd, typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.AndAnd, BoundBinaryOperatorType.ConditionalAnd, typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.Circumflex, BoundBinaryOperatorType.BitwiseXor, typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.EqualsEquals, BoundBinaryOperatorType.Equals, typeof(bool)),
            new BoundBinaryOperator(LanguageTokenType.ExclamationEquals, BoundBinaryOperatorType.NotEquals, typeof(bool)),
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