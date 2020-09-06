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
        BoundScope scope;

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnit compilationUnit)
        {
            BoundScope parentScope = CreateParentScope(previous);
            Diagnostics binderDiags = new Diagnostics(DateTime.Now);
            Binder binder = new Binder(binderDiags, parentScope);
            BoundStatement statement = binder.BindStatement(compilationUnit.statement);
            ImmutableArray<VariableSymbol> vars = binder.scope.GetDeclaredVariables();

            if (previous != null)
                binderDiags.Aggregate(previous.diagnostics);
            return new BoundGlobalScope(previous, binderDiags, vars, statement);
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

        private BoundStatement BindStatement(StatementNode statement)
        {
            switch (statement.nodeType)
            {
                case ParseNodeType.BlockStatement:
                    return BindBlockStatement((BlockStatement)statement);
                case ParseNodeType.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatement)statement);
                case ParseNodeType.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationStatement)statement);
                case ParseNodeType.IfStatement:
                    return BindIfStatement((IfStatement)statement);
                case ParseNodeType.WhileStatement:
                    return BindWhileStatement((WhileStatement)statement);
            }

            diagnostics.BinderError_UnexpectedStatementType(statement.nodeType, statement.textLocation.start);
            throw new Exception("Unable to bind unexpected statement type: " + statement.nodeType + " @" + statement.textLocation);
        }

        private BoundStatement BindBlockStatement(BlockStatement statement)
        {
            ImmutableArray<BoundStatement>.Builder statements = ImmutableArray.CreateBuilder<BoundStatement>();
            scope = new BoundScope(scope);
            
            foreach (StatementNode s in statement.statements)
            {
                statements.Add(BindStatement(s));
            }

            scope = scope.parent;
            return new BoundBlockStatement(statements.ToImmutable(), statement.textLocation);
        }

        private BoundStatement BindExpressionStatement(ExpressionStatement statement)
        {
            BoundExpression expression = BindExpression(statement.expression);
            return new BoundExpressionStatement(expression, statement.textLocation);
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationStatement statement)
        {
            BoundExpression initializer = BindExpression(statement.initializer);
            VariableSymbol variable = new VariableSymbol(statement.identifier.text, statement.isReadOnly, initializer.boundType);

            if (!scope.TryDeclare(variable))
            {
                diagnostics.BinderError_VariableAlreadyDeclared(variable, statement.textLocation.start);
            }
            return new BoundVariableDeclaration(variable, initializer, statement.textLocation);
        }

        private BoundStatement BindIfStatement(IfStatement statement)
        {
            BoundExpression condition = BindExpression(statement.condition, typeof(bool));
            BoundStatement body = BindStatement(statement.bodyStatement);
            BoundStatement elseStatement = statement.elseClause == null ? null : BindStatement(statement.elseClause.statement);
            return new BoundIfStatement(condition, body, elseStatement, statement.textLocation);
        }

        private BoundStatement BindWhileStatement(WhileStatement statement)
        {
            BoundExpression condition = BindExpression(statement.condition, typeof(bool));
            BoundStatement body = BindStatement(statement.body);
            return new BoundWhileStatement(condition, body, statement.textLocation);
        }

        private BoundExpression BindExpression(ExpressionNode node, Type targetType)
        {
            BoundExpression result = BindExpression(node);
            if (result.boundType != targetType)
            {
                diagnostics.BinderError_CannotConvertExpressionType(targetType, result.boundType, node.textLocation.start);
            }
            return result;
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

            if (!scope.TryLookup(expression.token.text, out VariableSymbol variable))
            {
                diagnostics.BinderError_VariableUndeclared(expression.token.text, expression.textLocation.start);
                return boundExpr;
            }

            if (variable.isReadOnly)
            {
                diagnostics.BinderError_ReadonlyVariableAssignment(variable, expression.textLocation.start);
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