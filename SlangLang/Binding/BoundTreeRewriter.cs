using System;
using System.Collections.Immutable;

namespace SlangLang.Binding
{
    internal abstract class BoundTreeRewriter
    {
        public virtual BoundStatement RewriteStatement(BoundStatement node)
        {
            switch (node.nodeType)
            {
                case BoundNodeType.BlockStatement:
                    return RewriteBlockStatement((BoundBlockStatement)node);
                case BoundNodeType.ExpressionStatement:
                    return RewriteExpressionStatement((BoundExpressionStatement)node);
                case BoundNodeType.VariableDeclarationStatement:
                    return RewriteVariableDeclaration((BoundVariableDeclaration)node);
                case BoundNodeType.IfStatement:
                    return RewriteIfStatement((BoundIfStatement)node);
                case BoundNodeType.WhileStatement:
                    return RewriteWhileStatement((BoundWhileStatement)node);
                case BoundNodeType.ForStatement:
                    return RewriteForStatement((BoundForStatement)node);
                default:
                    throw new Exception("Unexpected bound statement type in rewriter: " + node.nodeType);
            }
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = null;

            for (int i = 0; i < node.statements.Length; i++)
            {
                BoundStatement prevStatement = node.statements[i];
                BoundStatement currStatement = RewriteStatement(prevStatement);
                if (prevStatement != currStatement)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.statements.Length);
                        for (int j = 0; j < i; j++)
                        {
                            builder.Add(node.statements[j]);
                        }
                    }
                }

                if (builder != null)
                {
                    builder.Add(currStatement);
                }
            }

            if (builder == null)
                return node;
            return new BoundBlockStatement(builder.MoveToImmutable(), node.textLocation);
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            BoundExpression expression = RewriteExpression(node.expression);
            if (expression == node.expression)
                return node;

            return new BoundExpressionStatement(expression, node.textLocation);
        }

        protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
        {
            BoundExpression initializer = RewriteExpression(node.initializer);
            if (initializer == node.initializer)
            {
                return node;
            }

            return new BoundVariableDeclaration(node.variable, initializer, node.textLocation);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            BoundExpression condition = RewriteExpression(node.condition);
            BoundStatement body = RewriteStatement(node.body);
            BoundStatement elseStatement = node.elseStatement == null ? null : RewriteStatement(node.elseStatement);
            if (condition == node.condition && body == node.body && elseStatement == node.elseStatement)
            {
                return node;
            }

            return new BoundIfStatement(condition, body, elseStatement, node.textLocation);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            BoundExpression condition = RewriteExpression(node.condition);
            BoundStatement body = RewriteStatement(node.body);
            if (condition == node.condition && body == node.body)
            { 
                return node;
            }

            return new BoundWhileStatement(condition, body, node.textLocation);
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            BoundStatement setupStatement = RewriteStatement(node.setupStatement);
            BoundExpression condition = RewriteExpression(node.condition);
            BoundStatement postStatement = RewriteStatement(node.postStatement);
            BoundStatement body = RewriteStatement(node.body);
            if (setupStatement == node.setupStatement && condition == node.condition && postStatement == node.postStatement && body == node.body)
            {
                return node;
            }

            return new BoundForStatement(setupStatement, condition, postStatement, body, node.textLocation);
        }

        public virtual BoundExpression RewriteExpression(BoundExpression node)
        {
            switch (node.nodeType)
            {
                case BoundNodeType.LiteralExpression:
                    return RewriteLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeType.VariableExpression:
                    return RewriteVariableExpression((BoundVariableExpression)node);
                case BoundNodeType.AssignmentExpression:
                    return RewriteAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeType.UnaryExpression:
                    return RewriteUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeType.BinaryExpression:
                    return RewriteBinaryExpression((BoundBinaryExpression)node);
                default:
                    throw new Exception("Unexpected bound expression type in rewriter: " + node.nodeType);
            }
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            BoundExpression expression = RewriteExpression(node.expression);
            if (expression == node.expression)
                return node;

            return new BoundAssignmentExpression(node.variable, expression, node.textLocation);
        }

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            BoundExpression operand = RewriteExpression(node.operand);
            if (operand == node.operand)
                return node;

            return new BoundUnaryExpression(node.op, operand, node.textLocation);
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            BoundExpression left = RewriteExpression(node.left);
            BoundExpression right = RewriteExpression(node.right);
            if (left == node.left && right == node.right)
                return node;

            return new BoundBinaryExpression(left, node.op, right, node.textLocation);
        }
    }
}