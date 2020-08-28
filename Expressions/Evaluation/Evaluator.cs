using System;
using SlangLang.Debugging;

namespace SlangLang.Expressions.Evaluation
{
    public sealed class Evaluator
    {
        ExpressionNode root;
        Diagnostics diagnostics;

        public Evaluator(Diagnostics diag, ExpressionNode rootNode)
        {
            root = rootNode;
            diagnostics = diag;
        }

        public void Evaluate()
        {
            Console.WriteLine("Evaluator says: " + EvaluateExpression(root));
        }

        private int EvaluateExpression(ExpressionNode node)
        {
            if (node is LiteralExpression lit)
                return (int)lit.value; //trusting it's just an int for now
            if (node is BinaryExpression bin)
            {
                int leftResult = EvaluateExpression(bin.leftNode);
                int rightResult = EvaluateExpression(bin.rightNode);

                if (bin.nodeType == ExpressionNodeType.Add)
                    return leftResult + rightResult;
                if (bin.nodeType == ExpressionNodeType.Sub)
                    return leftResult - rightResult;
                if (bin.nodeType == ExpressionNodeType.Mult)
                    return leftResult * rightResult;
                if (bin.nodeType == ExpressionNodeType.Div)
                    return leftResult / rightResult;
                diagnostics.AddFailure("Evaluator", "Unexpected binary operator in eval code. TODO?", TextLocation.NoLocation, DateTime.Now);
            }
            throw new Exception("Unexpected node in evaluator.");
        }
    }
}