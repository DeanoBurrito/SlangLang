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
                case ParseNodeType.Literal:
                    return BindLiteralExpression((LiteralExpression)node);
                case ParseNodeType.Unary:
                    return BindUnaryExpression((UnaryExpression)node);
                case ParseNodeType.Binary:
                    return BindBinaryExpression((BinaryExpression)node);
                case ParseNodeType.Name:
                    return BindNameExpression((NameExpression)node);
                case ParseNodeType.Assignment:
                    return BindAssignmentExpression((AssignmentExpression)node);
            }

            diagnostics.BinderError_UnexpectedExpressionType(node.nodeType, node.textLocation.start);
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
                diagnostics.BinderError_UnaryOperatorNotDefined(expression.token, boundOperand.boundType, expression.textLocation.start);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand, expression.textLocation);
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expression)
        {
            BoundExpression boundLeft = BindExpression(expression.leftNode);
            BoundExpression boundRight = BindExpression(expression.rightNode);
            BoundBinaryOperator boundOperator = BoundBinaryOperator.Bind(expression.token.tokenType, boundLeft.boundType, boundRight.boundType);
            if (boundOperator == null)
            {
                diagnostics.BinderError_BinaryOperatorNotDefined(expression.token, boundLeft.boundType, boundRight.boundType, expression.textLocation.start);
                return boundLeft;
            }
            return new BoundBinaryExpression(boundOperator, boundLeft, boundRight, expression.textLocation);
        }

        private BoundExpression BindNameExpression(NameExpression expression)
        {
            string name = expression.token.text;
            VariableSymbol variable = variables.Keys.FirstOrDefault(v => v.name == name);
            
            if (variable == null)
            {
                diagnostics.BinderError_VariableDoesNotExist(name, expression.textLocation.start);
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