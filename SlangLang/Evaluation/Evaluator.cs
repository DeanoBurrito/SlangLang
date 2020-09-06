using System;
using System.Collections.Generic;
using SlangLang.Parsing;
using SlangLang.Debug;
using SlangLang.Binding;

namespace SlangLang.Evaluation
{
    internal sealed class Evaluator
    {
        private BoundStatement root;
        private Diagnostics diagnostics;
        private Dictionary<VariableSymbol, object> variables;
        private object lastValue;

        public Evaluator(Diagnostics diag, BoundStatement rootStatement, Dictionary<VariableSymbol, object> variables)
        {
            root = rootStatement;
            diagnostics = diag;
            this.variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(root);
            return lastValue;
        }

        private void EvaluateStatement(BoundStatement statement)
        {
            switch (statement.nodeType)
            {
                case BoundNodeType.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement)statement);
                    return;
                case BoundNodeType.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    return;
                case BoundNodeType.VariableDeclarationStatement:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)statement);
                    return;
            }

            throw new Exception("Unexpected statement in evaluator");
        }

        private void EvaluateBlockStatement(BoundBlockStatement statement)
        {
            foreach (BoundStatement s in statement.statements)
                EvaluateStatement(s);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement)
        {
            lastValue = EvaluateExpression(statement.expression);
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration statement)
        {
            object value = EvaluateExpression(statement.initializer);
            variables[statement.variable] = value;
            lastValue = value;
        }

        private object EvaluateExpression(BoundExpression node)
        {   
            switch (node.nodeType)
            {
                case BoundNodeType.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeType.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)node);
                case BoundNodeType.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeType.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeType.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);
            }

            throw new Exception("Unexpected node in evaluator.");
        }

        private object EvaluateLiteralExpression(BoundLiteralExpression expr)
        {
            return expr.value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression expr)
        {
            return variables[expr.variable];
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression expr)
        {
            object value = EvaluateExpression(expr.expression);
            variables[expr.variable] = value;
            return value;
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression expr)
        {
            object operandResult = EvaluateExpression(expr.operand);
            Type operandType = operandResult.GetType();
            if (operandType == typeof(int))
            {
                int resultInt = (int)operandResult;
                switch (expr.op.unaryOperator)
                {
                    case BoundUnaryOperatorType.Negate:
                        return -resultInt;
                }
            }
            else if (operandType == typeof(bool))
            {
                bool resultBool = (bool)operandResult;
                switch (expr.op.unaryOperator)
                {
                    case BoundUnaryOperatorType.Not:
                        return !resultBool;
                }
            }

            diagnostics.EvaluatorError_UnexpectedUnaryOperator(expr.op, expr.textLocation.start);
            throw new Exception("Unexpected unary operator in evaluator!");
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression expr)
        {
            object leftResult = EvaluateExpression(expr.left);
            object rightResult = EvaluateExpression(expr.right);
            Type leftType = leftResult.GetType();
            Type rightType = rightResult.GetType();
            if (leftType == typeof(int) && rightType == typeof(int))
            {
                int leftInt = (int)leftResult;
                int rightInt = (int)rightResult;
                switch (expr.op.binaryOperator)
                {
                    case BoundBinaryOperatorType.Addition:
                        return leftInt + rightInt;
                    case BoundBinaryOperatorType.Subtract:
                        return leftInt - rightInt;
                    case BoundBinaryOperatorType.Multiplication:
                        return leftInt * rightInt;
                    case BoundBinaryOperatorType.Division:
                        return leftInt / rightInt;
                    case BoundBinaryOperatorType.LessThan:
                        return leftInt < rightInt;
                    case BoundBinaryOperatorType.LessThanOrEqual:
                        return leftInt <= rightInt;
                    case BoundBinaryOperatorType.GreaterThan:
                        return leftInt > rightInt;
                    case BoundBinaryOperatorType.GreaterThanOrEqual:
                        return leftInt >= rightInt;
                }
            }
            else if (leftType == typeof(bool) && rightType == typeof(bool))
            {
                bool leftBool = (bool)leftResult;
                bool rightBool = (bool)rightResult;
                switch (expr.op.binaryOperator)
                {
                    case BoundBinaryOperatorType.ConditionalOr:
                        return leftBool || rightBool;
                    case BoundBinaryOperatorType.ConditionalAnd:
                        return leftBool && rightBool;
                }
            }
            if (expr.op.binaryOperator == BoundBinaryOperatorType.Equals)
                return Equals(leftResult, rightResult);
            else if (expr.op.binaryOperator == BoundBinaryOperatorType.NotEquals)
                return !Equals(leftResult, rightResult);
            
            diagnostics.EvaluatorError_UnxpectedBinaryOperator(expr.op, expr.textLocation.start);
            throw new Exception("Unexpected binary operator in evaluator!");
        }
    }
}