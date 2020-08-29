using System;
using SlangLang.Debug;
using SlangLang.Binding;

namespace SlangLang.Evaluation
{
    internal sealed class Evaluator
    {
        BoundExpression root;
        Diagnostics diagnostics;

        public Evaluator(Diagnostics diag, BoundExpression rootNode)
        {
            root = rootNode;
            diagnostics = diag;
        }

        public void Evaluate()
        {
            Console.WriteLine("Evaluator says: " + EvaluateExpression(root));
        }

        private object EvaluateExpression(BoundExpression node)
        {   
            if (node is BoundLiteralExpression lit)
            {
                return lit.value;
            }
            if (node is BoundUnaryExpression unary)
            {
                object operandResult = EvaluateExpression(unary.operand);
                Type operandType = operandResult.GetType();
                if (operandType == typeof(int))
                {
                    int resultInt = (int)operandResult;
                    switch (unary.operatorType)
                    {
                        case BoundUnaryOperatorType.Negate:
                            return -resultInt;
                    }
                }
                else if (operandType == typeof(bool))
                {
                    bool resultBool = (bool)operandResult;
                    switch (unary.operatorType)
                    {
                        case BoundUnaryOperatorType.Not:
                            return !resultBool;
                    }
                }

                diagnostics.AddFailure("Evaluator", "Unexpected unary operator: " + unary.nodeType, TextLocation.NoLocation, DateTime.Now);
            }
            if (node is BoundBinaryExpression bin)
            {
                object leftResult = EvaluateExpression(bin.left);
                object rightResult = EvaluateExpression(bin.right);
                Type leftType = leftResult.GetType();
                Type rightType = rightResult.GetType();
                if (leftType == typeof(int) && rightType == typeof(int))
                {
                    int leftInt = (int)leftResult;
                    int rightInt = (int)rightResult;
                    switch (bin.operatorType)
                    {
                        case BoundBinaryOperatorType.Addition:
                            return leftInt + rightInt;
                        case BoundBinaryOperatorType.Subtract:
                            return leftInt - rightInt;
                        case BoundBinaryOperatorType.Multiplication:
                            return leftInt * rightInt;
                        case BoundBinaryOperatorType.Division:
                            return leftInt * rightInt;
                    }
                }
                else if (leftType == typeof(bool) && rightType == typeof(bool))
                {
                    bool leftBool = (bool)leftResult;
                    bool rightBool = (bool)rightResult;
                    switch (bin.operatorType)
                    {
                        case BoundBinaryOperatorType.ConditionalOr:
                            return leftBool || rightBool;
                        case BoundBinaryOperatorType.ConditionalAnd:
                            return leftBool && rightBool;
                    }
                }
                
                diagnostics.AddFailure("Evaluator", "Unexpected binary operator in syntax tree: " + bin.nodeType, TextLocation.NoLocation, DateTime.Now);
            }

            throw new Exception("Unexpected node in evaluator.");
        }
    }
}