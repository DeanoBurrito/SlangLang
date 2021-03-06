using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SlangLang.Debug;
using SlangLang.Parsing;
using SlangLang.Symbols;

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

            BoundScope parent = CreateRootScope();
            while (stack.Count > 0)
            {
                previous = stack.Pop();
                BoundScope scope = new BoundScope(parent);
                foreach (VariableSymbol var in previous.variables)
                {
                    scope.TryDeclareVariable(var);
                }
                parent = scope;
            }
            return parent;
        }

        private static BoundScope CreateRootScope()
        {
            BoundScope result = new BoundScope(null);
            foreach (FunctionSymbol f in BuildInFunctions.GetAll())
            {
                result.TryDeclareFunction(f);
            }
            return result;
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
                case ParseNodeType.ForStatement:
                    return BindForStatement((ForStatement)statement);
            }

            diagnostics.BinderError_UnexpectedStatementType(statement.nodeType, statement.textLocation);
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
            BoundExpression expression = BindExpression(statement.expression, true);
            return new BoundExpressionStatement(expression, statement.textLocation);
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationStatement statement)
        {
            BoundExpression initializer = BindExpression(statement.initializer);
            VariableSymbol variable = BindVariable(statement.identifier?.value, statement.isReadOnly, initializer.boundType, 
                new TextSpan(statement.keyword.textLocation.start, statement.identifier.textLocation.end));

            return new BoundVariableDeclaration(variable, initializer, statement.textLocation);
        }

        private BoundStatement BindIfStatement(IfStatement statement)
        {
            BoundExpression condition = BindExpression(statement.condition, TypeSymbol.Bool);
            BoundStatement body = BindStatement(statement.bodyStatement);
            BoundStatement elseStatement = statement.elseClause == null ? null : BindStatement(statement.elseClause.statement);
            return new BoundIfStatement(condition, body, elseStatement, statement.textLocation);
        }

        private BoundStatement BindWhileStatement(WhileStatement statement)
        {
            BoundExpression condition = BindExpression(statement.condition, TypeSymbol.Bool);
            BoundStatement body = BindStatement(statement.body);
            return new BoundWhileStatement(condition, body, statement.textLocation);
        }

        private BoundStatement BindForStatement(ForStatement statement)
        {
            BoundStatement setup = BindStatement(statement.setupStatement);
            BoundExpression condition = BindExpression(statement.condition, TypeSymbol.Bool);
            BoundStatement post = BindStatement(statement.postStatement);
            BoundStatement body = BindStatement(statement.body);
            return new BoundForStatement(setup, condition, post, body, statement.textLocation);
        }

        private BoundExpression BindExpression(ExpressionNode node, bool canBeVoid = false)
        {
            BoundExpression result = BindExpressionInternal(node);
            if (!canBeVoid && result.boundType == TypeSymbol.Void)
            {
                diagnostics.BinderError_ExpressionMustReturnValue(node.textLocation);
                return new BoundErrorExpression(node.textLocation);
            }
            
            return result;
        }

        private BoundExpression BindExpression(ExpressionNode node, TypeSymbol targetType)
        {
            BoundExpression result = BindExpression(node);
            if (result.boundType != targetType && result.boundType != TypeSymbol.Error && targetType != TypeSymbol.Error)
            {
                diagnostics.BinderError_CannotConvertExpressionType(targetType, result.boundType, node.textLocation);
            }
            return result;
        }

        private BoundExpression BindExpressionInternal(ExpressionNode node)
        {
            switch (node.nodeType)
            {
                case ParseNodeType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpression)node);
                case ParseNodeType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpression)node);
                case ParseNodeType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpression)node);
                case ParseNodeType.NameExpression:
                    return BindNameExpression((NameExpression)node);
                case ParseNodeType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpression)node);
                case ParseNodeType.CallExpression:
                    return BindCallExpression((CallExpression)node);
            }

            diagnostics.BinderError_UnexpectedExpressionType(node.nodeType, node.textLocation);
            throw new Exception("Unable to bind on unexpected expresson node: " + node.nodeType + " @" + node.textLocation.ToString());
        }

        private BoundExpression BindLiteralExpression(LiteralExpression expression)
        {
            object val = expression.value == null ? 0 : expression.value; //TODO: try and bind type based on the literal used
            //TODO: add a way to get the default value for a type.
            TypeSymbol type;
            switch (expression.token.tokenType)
            {
                case LanguageTokenType.String:
                    type = TypeSymbol.String;
                    break;
                case LanguageTokenType.IntegerNumber:
                    type = TypeSymbol.Int;
                    break;
                case LanguageTokenType.KeywordTrue:
                case LanguageTokenType.KeywordFalse:
                    type = TypeSymbol.Bool;
                    break;
                default:
                    throw new Exception("Unable to bind type of literal.");
            }

            return new BoundLiteralExpression(val, type, expression.textLocation);
        }

        private BoundExpression BindUnaryExpression(UnaryExpression expression)
        {
            BoundExpression boundOperand = BindExpression(expression.operand);
            if (boundOperand.nodeType == BoundNodeType.ErrorExpression)
                return new BoundErrorExpression(boundOperand.textLocation);

            BoundUnaryOperator boundOperator = BoundUnaryOperator.Bind(expression.token.tokenType, boundOperand.boundType);
            if (boundOperator == null)
            {
                diagnostics.BinderError_UnaryOperatorNotDefined(expression.token, boundOperand.boundType, expression.textLocation);
                return new BoundErrorExpression(expression.textLocation);
            }

            return new BoundUnaryExpression(boundOperator, boundOperand, expression.textLocation);
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expression)
        {
            BoundExpression boundLeft = BindExpression(expression.leftNode);
            BoundExpression boundRight = BindExpression(expression.rightNode);
            if (boundLeft.nodeType == BoundNodeType.ErrorExpression || boundRight.nodeType == BoundNodeType.ErrorExpression)
                return new BoundErrorExpression(new TextSpan(boundLeft.textLocation.start, boundRight.textLocation.end));

            BoundBinaryOperator boundOperator = BoundBinaryOperator.Bind(expression.token.tokenType, boundLeft.boundType, boundRight.boundType);
            if (boundOperator == null)
            {
                diagnostics.BinderError_BinaryOperatorNotDefined(expression.token, boundLeft.boundType, boundRight.boundType, expression.textLocation);
                return new BoundErrorExpression(expression.textLocation);
            }
            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight, expression.textLocation);
        }

        private BoundExpression BindNameExpression(NameExpression expression)
        {
            string name = expression.token.value;
            if (string.IsNullOrEmpty(name)) //inserted by parser, error has already been reported.
                return new BoundErrorExpression(expression.textLocation);
            
            if (!scope.TryLookupVariable(name, out VariableSymbol variable))
            {
                diagnostics.BinderError_VariableDoesNotExist(name, expression.textLocation);
                return new BoundErrorExpression(expression.textLocation);
            }

            return new BoundVariableExpression(variable, expression.textLocation);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression expression)
        {
            BoundExpression boundExpr = BindExpression(expression.expression);

            if (!scope.TryLookupVariable(expression.token.value, out VariableSymbol variable))
            {
                diagnostics.BinderError_VariableUndeclared(expression.token.value, expression.token.textLocation);
                return boundExpr;
            }

            if (variable.isReadOnly)
            {
                diagnostics.BinderError_ReadonlyVariableAssignment(variable, expression.textLocation);
            }
            
            if (boundExpr.boundType != variable.type)
            {
                diagnostics.BinderError_CannotCastVariable(variable, boundExpr.boundType, expression.textLocation);
                return boundExpr;
            }

            return new BoundAssignmentExpression(variable, boundExpr, expression.textLocation);
        }

        private BoundExpression BindCallExpression(CallExpression expression)
        {
            if (expression.arguments.count == 1 && LookupType(expression.token.value) is TypeSymbol type)
            {
                return BindConversion(type, expression.arguments[0]);
            }
            
            ImmutableArray<BoundExpression>.Builder boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();
            foreach (ExpressionNode arg in expression.arguments)
            {
                boundArguments.Add(BindExpression(arg));
            }

            if (!scope.TryLookupFunction(expression.token.value, out FunctionSymbol function))
            {
                diagnostics.BinderError_UndefinedFunction(expression.token.value, expression.token.textLocation);
                return new BoundErrorExpression(expression.textLocation);
            }

            if (expression.arguments.count != function.parameters.Length)
            {
                diagnostics.BinderError_MismatchedArgumentCount(function.name, expression.arguments.count, function.parameters.Length, expression.textLocation);
                return new BoundErrorExpression(expression.textLocation);
            }

            for (int i = 0; i < expression.arguments.count; i++)
            {
                BoundExpression argument = boundArguments[i];
                ParameterSymbol parameter = function.parameters[i];

                if (argument.boundType != parameter.type)
                {
                    diagnostics.BinderError_ArgumentTypeMismatch(argument.boundType, parameter.type, i, expression.arguments[i].textLocation);
                    return new BoundErrorExpression(expression.arguments[i].textLocation);
                }
            }

            return new BoundCallExpression(function, boundArguments.ToImmutable(), expression.textLocation);
        }

        private BoundExpression BindConversion(TypeSymbol type, ExpressionNode expression)
        {
            BoundExpression boundExpression = BindExpression(expression);
            Conversion conversion = Conversion.Classify(boundExpression.boundType, type);
            if (!conversion.exists)
            {
                diagnostics.BinderError_CannotConvertExpressionType(type, boundExpression.boundType, boundExpression.textLocation);
                return new BoundErrorExpression(TextSpan.NoText);
            }

            return new BoundConversionExpression(type, boundExpression, expression.textLocation);
        }

        private VariableSymbol BindVariable(string identifier, bool isReadOnly, TypeSymbol type, TextSpan where)
        {
            string name = identifier ?? "?";

            VariableSymbol variable = new VariableSymbol(name, isReadOnly, type);
            if (!scope.TryDeclareVariable(variable))
                diagnostics.BinderError_SymbolAlreadyDeclared(variable, where);
            return variable;
        }

        private TypeSymbol LookupType(string name)
        {
            switch (name)
            {
                case "bool":
                    return TypeSymbol.Bool;
                case "int":
                    return TypeSymbol.Int;
                case "string":
                    return TypeSymbol.String;
                default:
                    return null;
            }
        }
    }
}