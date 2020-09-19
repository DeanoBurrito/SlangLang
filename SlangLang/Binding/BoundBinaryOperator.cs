using System;
using SlangLang.Parsing;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class BoundBinaryOperator
    {
        public readonly LanguageTokenType langToken;
        public readonly BoundBinaryOperatorType binaryOperator;
        public readonly TypeSymbol leftOperandType;
        public readonly TypeSymbol rightOperandType;
        public readonly TypeSymbol resultType;
        
        private BoundBinaryOperator(LanguageTokenType token, BoundBinaryOperatorType binaryOp, TypeSymbol leftOpType, TypeSymbol rightOpType, TypeSymbol resultType)
        {
            langToken = token;
            binaryOperator = binaryOp;
            leftOperandType = leftOpType;
            rightOperandType = rightOpType;
            this.resultType = resultType;
        }

        private BoundBinaryOperator(LanguageTokenType tokenType, BoundBinaryOperatorType binaryOp, TypeSymbol type, TypeSymbol resultType) : this(tokenType, binaryOp, type, type, resultType)
        {}

        private BoundBinaryOperator(LanguageTokenType token, BoundBinaryOperatorType binaryOp, TypeSymbol type) : this(token, binaryOp, type, type, type)
        {}

        public override string ToString()
        {
            return binaryOperator + " (" + leftOperandType + ", " + rightOperandType + ") => " + resultType;
        }

        private static BoundBinaryOperator[] ops = 
        {
            new BoundBinaryOperator(LanguageTokenType.Plus, BoundBinaryOperatorType.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(LanguageTokenType.Minus, BoundBinaryOperatorType.Subtract, TypeSymbol.Int),
            new BoundBinaryOperator(LanguageTokenType.Star, BoundBinaryOperatorType.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(LanguageTokenType.ForwardSlash, BoundBinaryOperatorType.Division, TypeSymbol.Int),
            new BoundBinaryOperator(LanguageTokenType.Pipe, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Int),
            new BoundBinaryOperator(LanguageTokenType.And, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Int),
            new BoundBinaryOperator(LanguageTokenType.Circumflex, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Int),

            new BoundBinaryOperator(LanguageTokenType.EqualsEquals, BoundBinaryOperatorType.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.ExclamationEquals, BoundBinaryOperatorType.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.Less, BoundBinaryOperatorType.LessThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.LessOrEquals, BoundBinaryOperatorType.LessThanOrEqual, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.Greater, BoundBinaryOperatorType.GreaterThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.GreaterOrEquals, BoundBinaryOperatorType.GreaterThanOrEqual, TypeSymbol.Int, TypeSymbol.Bool),

            new BoundBinaryOperator(LanguageTokenType.Pipe, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.PipePipe, BoundBinaryOperatorType.ConditionalOr, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.And, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.AndAnd, BoundBinaryOperatorType.ConditionalAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.Circumflex, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.EqualsEquals, BoundBinaryOperatorType.Equals, TypeSymbol.Bool),
            new BoundBinaryOperator(LanguageTokenType.ExclamationEquals, BoundBinaryOperatorType.NotEquals, TypeSymbol.Bool),

            new BoundBinaryOperator(LanguageTokenType.Plus, BoundBinaryOperatorType.Addition, TypeSymbol.String),
        };

        public static BoundBinaryOperator Bind(LanguageTokenType langTokenType, TypeSymbol leftType, TypeSymbol rightType)
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