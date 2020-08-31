using System;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class Binder
    {
        readonly Diagnostics diagnostics;
        readonly ExpressionNode rootNode;

        readonly Dictionary<string, object> variables;

        public Binder(Diagnostics diag, ExpressionNode root, Dictionary<string, object> availableVars)
        {
            diagnostics = diag;
            rootNode = root;
            variables = availableVars;
        }

        public BoundExpression BindAll()
        {
            BoundExpression expr = BindExpression(rootNode);
            return expr;
        }

        private BoundExpression BindExpression(ExpressionNode node)
        {
            switch (node.nodeType)
            {
                case ExpressionNodeType.Literal:
                    return BindLiteralExpression((LiteralExpression)node);
                case ExpressionNodeType.Unary:
                    return BindUnaryExpression((UnaryExpression)node);
                case ExpressionNodeType.Binary:
                    return BindBinaryExpression((BinaryExpression)node);
                case ExpressionNodeType.Name:
                    return BindNameExpression((NameExpression)node);
                case ExpressionNodeType.Assignment:
                    return BindAssignmentExpression((AssignmentExpression)node);
            }

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
            BoundUnaryOperator boundOperator = BoundUnaryOperator.Bind(expression.opToken.tokenType, boundOperand.boundType);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.opToken} is not defined for type {boundOperand.boundType}.", expression.textLocation, DateTime.Now);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand, expression.textLocation);
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expression)
        {
            BoundExpression boundLeft = BindExpression(expression.leftNode);
            BoundExpression boundRight = BindExpression(expression.rightNode);
            BoundBinaryOperator boundOperator = BoundBinaryOperator.Bind(expression.opToken.tokenType, boundLeft.boundType, boundRight.boundType);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.opToken} is not defined for types {boundLeft.boundType}, {boundRight.boundType}.", expression.textLocation, DateTime.Now);
                return boundLeft;
            }
            return new BoundBinaryExpression(boundOperator, boundLeft, boundRight, expression.textLocation);
        }

        private BoundExpression BindNameExpression(NameExpression expression)
        {
            string name = expression.identifierToken.text;
            if (!variables.TryGetValue(expression.identifierToken.text, out object nameRef))
            {
                diagnostics.AddFailure("Binder", "Unable to bind variable " + name + ", it does not exist.", expression.textLocation, DateTime.Now);
                return new BoundLiteralExpression(0, TextLocation.NoLocation); //just return int(0) so the tree dosnt crash
            }

            Type nameType = nameRef.GetType();
            return new BoundVariableExpression(name, nameType, expression.textLocation);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression expression)
        {
            BoundExpression boundExpr = BindExpression(expression.expression);

            object defaultValue = boundExpr.boundType == typeof(int)
                ? 0
                : boundExpr.boundType == typeof(bool)
                    ? (object)false 
                    : null;
                if (defaultValue == null)
                {
                    throw new Exception("Unhandled variable type. Type requested: " + boundExpr.boundType);
                }
            variables[expression.identiferToken.text] = defaultValue;

            return new BoundAssignmentExpression(expression.identiferToken.text, boundExpr, expression.textLocation);
        }
    }
}