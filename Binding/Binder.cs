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
            BoundUnaryOperator boundOperator = BoundUnaryOperator.Bind(expression.nodeType, boundOperand.boundType);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.nodeType} is not defined for type {boundOperand.boundType}.", expression.textLocation, DateTime.Now);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand, expression.textLocation);
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expression)
        {
            BoundExpression boundLeft = BindExpression(expression.leftNode);
            BoundExpression boundRight = BindExpression(expression.rightNode);
            BoundBinaryOperator boundOperator = BoundBinaryOperator.Bind(expression.nodeType, boundLeft.boundType, boundRight.boundType);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.nodeType} is not defined for types {boundLeft.boundType}, {boundRight.boundType}.", expression.textLocation, DateTime.Now);
                return boundLeft;
            }
            return new BoundBinaryExpression(boundOperator, boundLeft, boundRight, expression.textLocation);
        }
    }
}