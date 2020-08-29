using System;
using SlangLang.Debug;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class Binder
    {
        readonly Diagnostics diagnostics;
        readonly ExpressionNode rootNode;

        public Binder(Diagnostics diag, ExpressionNode root)
        {
            diagnostics = diag;
            rootNode = root;
        }

        public BoundExpression BindAll()
        {
            BoundExpression expr = BindExpression(rootNode);
            return expr;
        }
        
        private BoundExpression BindExpression(ExpressionNode node)
        {
            if (node is LiteralExpression l)
                return BindLiteralExpression(l);
            else if (node is UnaryExpression u)
                return BindUnaryExpression(u);
            else if (node is BinaryExpression b)
                return BindBinaryExpression(b);

            diagnostics.AddFailure("Binder", "Unexpected expression type to bind: " + node.nodeType, node.textLocation, DateTime.Now);
            throw new Exception("Unable to bind on unexpected expresson node: " + node.nodeType + " @" + node.textLocation.ToString());
        }

        private BoundExpression BindLiteralExpression(LiteralExpression expression)
        {
            object val = expression.value == null ? 0 : expression.value; //TODO: move this away from just integer literals
            return new BoundLiteralExpression(val, expression.textLocation);
        }

        private BoundExpression BindUnaryExpression(UnaryExpression expression)
        {
            BoundExpression boundOperand = BindExpression(expression.operand);
            BoundUnaryOperatorType? boundOperator = BindUnaryOperator(expression.nodeType, boundOperand.boundType,expression.textLocation);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.nodeType} is not defined for type {boundOperand.boundType}.", expression.textLocation, DateTime.Now);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator.Value, boundOperand, expression.textLocation);
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expression)
        {
            BoundExpression boundLeft = BindExpression(expression.leftNode);
            BoundExpression boundRight = BindExpression(expression.rightNode);
            BoundBinaryOperatorType? boundOperator = BindBinaryOperator(expression.nodeType, boundLeft.boundType, boundRight.boundType, expression.textLocation);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.nodeType} is not defined for types {boundLeft.boundType}, {boundRight.boundType}.", expression.textLocation, DateTime.Now);
                return boundLeft;
            }
            return new BoundBinaryExpression(boundOperator.Value, boundLeft, boundRight, expression.textLocation);
        }

        private BoundUnaryOperatorType? BindUnaryOperator(ExpressionNodeType op, Type operandType, TextLocation where)
        {
            if (operandType == typeof(int))
            {
                switch (op)
                {
                    case ExpressionNodeType.Negate:
                        return BoundUnaryOperatorType.Negate;
                }
            }
            if (operandType == typeof(bool))
            {
                switch (op)
                {
                    case ExpressionNodeType.Not:
                        return BoundUnaryOperatorType.Not;
                }
            }
            return null;
        }

        private BoundBinaryOperatorType? BindBinaryOperator(ExpressionNodeType op, Type leftType, Type rightType, TextLocation where)
        {   
            if (leftType == typeof(int) && rightType == typeof(int))
            {
                switch (op)
                {
                    case ExpressionNodeType.Addition:
                        return BoundBinaryOperatorType.Addition;
                    case ExpressionNodeType.Subtraction:
                        return BoundBinaryOperatorType.Subtract;
                    case ExpressionNodeType.Multiplication:
                        return BoundBinaryOperatorType.Multiplication;
                    case ExpressionNodeType.Division:
                        return BoundBinaryOperatorType.Division;
                }
            }
            if (leftType == typeof(bool) && rightType == typeof(bool))
            {
                switch (op)
                {
                    case ExpressionNodeType.ConditionalOr:
                        return BoundBinaryOperatorType.ConditionalOr;
                    case ExpressionNodeType.ConditionalAnd:
                        return BoundBinaryOperatorType.ConditionalAnd;
                }
            }
            return null;
        }
    }
}