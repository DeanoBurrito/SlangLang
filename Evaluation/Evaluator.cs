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

        private int EvaluateExpression(BoundExpression node)
        {   
            if (node is BoundLiteralExpression lit)
                return (int)lit.value; //trusting it's just an int for now
            if (node is BoundUnaryExpression unary)
            {
                int operandResult = EvaluateExpression(unary.operand);
                switch (unary.operatorType)
                {
                    case BoundUnaryOperatorType.Negate:
                        return -operandResult;
                }
                diagnostics.AddFailure("Evaluator", "Unexpected unary operator: " + unary.nodeType, TextLocation.NoLocation, DateTime.Now);
            }
            if (node is BoundBinaryExpression bin)
            {
                int leftResult = EvaluateExpression(bin.left);
                int rightResult = EvaluateExpression(bin.right);
                switch (bin.operatorType)
                {
                    case BoundBinaryOperatorType.Addition:
                        return leftResult + rightResult;
                    case BoundBinaryOperatorType.Subtract:
                        return leftResult - rightResult;
                    case BoundBinaryOperatorType.Multiplication:
                        return leftResult * rightResult;
                    case BoundBinaryOperatorType.Division:
                        return leftResult / rightResult;
                }
                diagnostics.AddFailure("Evaluator", "Unexpected binary operator in syntax tree: " + bin.nodeType, TextLocation.NoLocation, DateTime.Now);
            }
            throw new Exception("Unexpected node in evaluator.");
        }
    }
}