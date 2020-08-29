using System;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class BoundUnaryOperator
    {
        public readonly ExpressionNodeType nodeType;
        public readonly BoundUnaryOperatorType unaryOperator;
        public readonly Type operandType;
        public readonly Type resultType;
        
        private BoundUnaryOperator(ExpressionNodeType nodeT, BoundUnaryOperatorType unaryOp, Type operandType, Type resultType)
        {
            nodeType = nodeT;
            unaryOperator = unaryOp;
            this.operandType = operandType;
            this.resultType = resultType;
        }

        private BoundUnaryOperator(ExpressionNodeType nodeT, BoundUnaryOperatorType unaryOp, Type operandType) : this(nodeT, unaryOp, operandType, operandType)
        {}

        public override string ToString()
        {
            return unaryOperator + " (" + operandType + ") => " + resultType;
        }

        private static BoundUnaryOperator[] ops = 
        {
            new BoundUnaryOperator(ExpressionNodeType.Negate, BoundUnaryOperatorType.Negate, typeof(int)),
            
            new BoundUnaryOperator(ExpressionNodeType.Not, BoundUnaryOperatorType.Not, typeof(bool)),
        };

        public static BoundUnaryOperator Bind(ExpressionNodeType opType, Type operandType)
        {
            foreach (BoundUnaryOperator op in ops)
            {
                if (op.nodeType == opType && op.operandType == operandType)
                    return op;
            }
            return null;
        }
    }
}