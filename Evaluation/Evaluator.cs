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
                if (operandResult.GetType() == typeof(int))
                {
                    int result = (int)operandResult;
                    switch (unary.operatorType)
                    {
                        case BoundUnaryOperatorType.Negate:
                            return -result;
                    }
                }

                diagnostics.AddFailure("Evaluator", "Unexpected unary operator: " + unary.nodeType, TextLocation.NoLocation, DateTime.Now);
            }
            if (node is BoundBinaryExpression bin)
            {
                object leftResult = EvaluateExpression(bin.left);
                object rightResult = EvaluateExpression(bin.right);
                if (leftResult.GetType() == typeof(int) && rightResult.GetType() == typeof(int))
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
                
                diagnostics.AddFailure("Evaluator", "Unexpected binary operator in syntax tree: " + bin.nodeType, TextLocation.NoLocation, DateTime.Now);
            }

            throw new Exception("Unexpected node in evaluator.");
        }
    }
}