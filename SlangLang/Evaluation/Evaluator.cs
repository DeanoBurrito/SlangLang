using System;
using System.Collections.Generic;
using SlangLang.Parsing;
using SlangLang.Debug;
using SlangLang.Binding;

namespace SlangLang.Evaluation
{
    internal sealed class Evaluator
    {
        BoundExpression root;
        Diagnostics diagnostics;
        Dictionary<VariableSymbol, object> variables;

        public Evaluator(Diagnostics diag, BoundExpression rootNode, Dictionary<VariableSymbol, object> variables)
        {
            root = rootNode;
            diagnostics = diag;
            this.variables = variables;
        }

        public object Evaluate()
        {
            return EvaluateExpression(root);
        }

        private object EvaluateExpression(BoundExpression node)
        {   
            if (node is BoundLiteralExpression lit)
            {
                return lit.value;
            }
            if (node is BoundVariableExpression var)
            {
                return variables[var.variable];
            }
            if (node is BoundAssignmentExpression assign) //'ass' haha almost.
            {
                object value = EvaluateExpression(assign.expression);
                variables[assign.variable] = value;
                return value;
            }
            if (node is BoundUnaryExpression unary)
            {
                object operandResult = EvaluateExpression(unary.operand);
                Type operandType = operandResult.GetType();
                if (operandType == typeof(int))
                {
                    int resultInt = (int)operandResult;
                    switch (unary.op.unaryOperator)
                    {
                        case BoundUnaryOperatorType.Negate:
                            return -resultInt;
                    }
                }
                else if (operandType == typeof(bool))
                {
                    bool resultBool = (bool)operandResult;
                    switch (unary.op.unaryOperator)
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
                    switch (bin.op.binaryOperator)
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
                    switch (bin.op.binaryOperator)
                    {
                        case BoundBinaryOperatorType.ConditionalOr:
                            return leftBool || rightBool;
                        case BoundBinaryOperatorType.ConditionalAnd:
                            return leftBool && rightBool;
                    }
                }
                if (bin.op.binaryOperator == BoundBinaryOperatorType.Equals)
                    return Equals(leftResult, rightResult);
                else if (bin.op.binaryOperator == BoundBinaryOperatorType.NotEquals)
                    return !Equals(leftResult, rightResult);
                
                diagnostics.AddFailure("Evaluator", "Unexpected binary operator in syntax tree: " + bin.nodeType, TextLocation.NoLocation, DateTime.Now);
            }

            throw new Exception("Unexpected node in evaluator.");
        }
    }
}