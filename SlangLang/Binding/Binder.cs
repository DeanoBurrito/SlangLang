using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SlangLang.Debug;
using SlangLang.Parsing;

namespace SlangLang.Binding
{
    internal sealed class Binder
    {
        readonly Diagnostics diagnostics;
        readonly BoundScope scope;

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnit compilationUnit)
        {
            BoundScope parentScope = CreateParentScope(previous);
            Diagnostics binderDiags = new Diagnostics(DateTime.Now);
            Binder binder = new Binder(binderDiags, parentScope);
            BoundExpression expression = binder.BindExpression(compilationUnit.expression);
            ImmutableArray<VariableSymbol> vars = binder.scope.GetDeclaredVariables();

            if (previous != null)
                binderDiags.Aggregate(previous.diagnostics);
            return new BoundGlobalScope(previous, binderDiags, vars, expression);
        }

        private static BoundScope CreateParentScope(BoundGlobalScope previous)
        {
            Stack<BoundGlobalScope> stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.previous;
            }

            BoundScope parent = null;
            while (stack.Count > 0)
            {
                previous = stack.Pop();
                BoundScope scope = new BoundScope(parent);
                foreach (VariableSymbol var in previous.variables)
                {
                    scope.TryDeclare(var);
                }
                parent = scope;
            }
            return parent;
        }

        public Binder(Diagnostics diag, BoundScope parentScope)
        {
            diagnostics = diag;
            scope = new BoundScope(parentScope);
        }

        public BoundExpression BindExpression(ExpressionNode node)
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
            
            if (!scope.TryLookup(name, out VariableSymbol variable))
            {
                diagnostics.BinderError_VariableDoesNotExist(name, expression.textLocation.start);
                return new BoundLiteralExpression(0, TextSpan.NoText); //just return int(0) so the tree dosnt crash
            }

            return new BoundVariableExpression(variable, expression.textLocation);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression expression)
        {
            BoundExpression boundExpr = BindExpression(expression.expression);
            VariableSymbol variable = new VariableSymbol(expression.token.text, boundExpr.boundType); 

            if (!scope.TryLookup(expression.token.text, out variable))
            {
                variable = new VariableSymbol(expression.token.text, boundExpr.boundType);
                scope.TryDeclare(variable);
            }
            
            if (boundExpr.boundType != variable.type)
            {
                diagnostics.BinderError_CannotCastVariable(variable, boundExpr.boundType, expression.textLocation.start);
                return boundExpr;
            }

            return new BoundAssignmentExpression(variable, boundExpr, expression.textLocation);
        }
    }
}