using System;
using System.Linq;
using System.Collections.Generic;
using SlangLang.Debug;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class Binder
    {
        readonly Diagnostics diagnostics;
        readonly ExpressionNode rootNode;

        readonly Dictionary<VariableSymbol, object> variables;

        public Binder(Diagnostics diag, ExpressionNode root, Dictionary<VariableSymbol, object> availableVars)
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

            diagnostics.AddFailure("Binder", "Unexpected expression type to bind: " + node.nodeType, node.textLocation.start, DateTime.Now);
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
            BoundUnaryOperator boundOperator = BoundUnaryOperator.Bind(expression.token.tokenType, boundOperand.boundType);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.token} is not defined for type {boundOperand.boundType}.", expression.textLocation.start, DateTime.Now);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand, new TextSpan(boundOperand.textLocation.start, boundOperand.textLocation.end));
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expression)
        {
            BoundExpression boundLeft = BindExpression(expression.leftNode);
            BoundExpression boundRight = BindExpression(expression.rightNode);
            BoundBinaryOperator boundOperator = BoundBinaryOperator.Bind(expression.token.tokenType, boundLeft.boundType, boundRight.boundType);
            if (boundOperator == null)
            {
                diagnostics.AddFailure("Binder", $"Unary operator {expression.token} is not defined for types {boundLeft.boundType}, {boundRight.boundType}.", expression.textLocation.start, DateTime.Now);
                return boundLeft;
            }
            return new BoundBinaryExpression(boundOperator, boundLeft, boundRight, new TextSpan(boundLeft.textLocation.start, boundRight.textLocation.end));
        }

        private BoundExpression BindNameExpression(NameExpression expression)
        {
            string name = expression.token.text;
            VariableSymbol variable = variables.Keys.FirstOrDefault(v => v.name == name);
            
            if (variable == null)
            {
                diagnostics.AddFailure("Binder", "Unable to bind variable " + name + ", it does not exist.", expression.textLocation.start, DateTime.Now);
                return new BoundLiteralExpression(0, TextSpan.NoText); //just return int(0) so the tree dosnt crash
            }

            return new BoundVariableExpression(variable, expression.textLocation);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression expression)
        {
            BoundExpression boundExpr = BindExpression(expression.expression);

            VariableSymbol existingVariable = variables.Keys.FirstOrDefault(v => v.name == expression.token.text);
            if (existingVariable != null)
                variables.Remove(existingVariable);
            
            VariableSymbol variable = new VariableSymbol(expression.token.text, boundExpr.boundType); 
            variables[variable] = null;

            return new BoundAssignmentExpression(variable, boundExpr, expression.textLocation);
        }
    }
}