using System;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class BoundBinaryOperator
    {
        public readonly ExpressionNodeType nodeType;
        public readonly BoundBinaryOperatorType binaryOperator;
        public readonly Type leftOperandType;
        public readonly Type rightOperandType;
        public readonly Type resultType;
        
        private BoundBinaryOperator(ExpressionNodeType nodeT, BoundBinaryOperatorType binaryOp, Type leftOpType, Type rightOpType, Type resultType)
        {
            nodeType = nodeT;
            binaryOperator = binaryOp;
            leftOperandType = leftOpType;
            rightOperandType = rightOpType;
            this.resultType = resultType;
        }

        private BoundBinaryOperator(ExpressionNodeType nodeType, BoundBinaryOperatorType binaryOp, Type type) : this(nodeType, binaryOp, type, type, type)
        {}

        public override string ToString()
        {
            return binaryOperator + " (" + leftOperandType + ", " + rightOperandType + ") => " + resultType;
        }

        private static BoundBinaryOperator[] ops = 
        {
            new BoundBinaryOperator(ExpressionNodeType.Addition, BoundBinaryOperatorType.Addition, typeof(int)),
            new BoundBinaryOperator(ExpressionNodeType.Subtraction, BoundBinaryOperatorType.Subtract, typeof(int)),
            new BoundBinaryOperator(ExpressionNodeType.Multiplication, BoundBinaryOperatorType.Multiplication, typeof(int)),
            new BoundBinaryOperator(ExpressionNodeType.Division, BoundBinaryOperatorType.Division, typeof(int)),

            new BoundBinaryOperator(ExpressionNodeType.ConditionalOr, BoundBinaryOperatorType.ConditionalOr, typeof(bool)),
            new BoundBinaryOperator(ExpressionNodeType.ConditionalAnd, BoundBinaryOperatorType.ConditionalAnd, typeof(bool)),
        };

        public static BoundBinaryOperator Bind(ExpressionNodeType opType, Type leftType, Type rightType)
        {
            foreach (BoundBinaryOperator op in ops)
            {
                if (op.nodeType == opType && op.leftOperandType == leftType && op.rightOperandType == rightType)
                    return op;
            }
            return null;
        }
    }
}