using System;
using SlangLang.Parsing;
using SlangLang.Binding;
using SlangLang.Symbols;

namespace SlangLang.Debug
{
    internal static class DiagnosticsWriteProxy
    {
        internal static void LexerError_ExpectedEndOfStringLiteral(this Diagnostics diagnostics, string foundInstead, TextSpan where)
        {
            diagnostics.AddFailure("Lexer", $"Expected \" to end string literal, found {foundInstead} instead.", where, DateTime.Now);
        }

        internal static void LexerError_GotBadInput(this Diagnostics diagnostics, string found, TextSpan where)
        {
            diagnostics.AddFailure("Lexer", $"Bad character in input text, found {found}" + ".", where, DateTime.Now);
        }

        internal static void ParserError_CouldNotParseInt(this Diagnostics diagnostics, TextSpan where)
        {
            diagnostics.AddFailure("Parser", "Could not parse int from number token.", where, DateTime.Now);
        }

        internal static void ParserError_TokenMatchFailed(this Diagnostics diagnostics, LanguageTokenType expected, LanguageTokenType actual, TextSpan where)
        {
            diagnostics.AddFailure("Parser", "Token match failed, expected " + expected + ", found " + actual + " instead.", where, DateTime.Now);
        }

        internal static void BinderError_UnexpectedStatementType(this Diagnostics diagnostics, ParseNodeType type, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Failed to bind statement, unexpected type " + type + ".", where, DateTime.Now);
        }

        internal static void BinderError_UnexpectedExpressionType(this Diagnostics diagnostics, ParseNodeType type, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Failed to bind expression in tree, unexpected type " + type + ".", where, DateTime.Now);
        }

        internal static void BinderError_UnaryOperatorNotDefined(this Diagnostics diagnostics, LanguageToken opToken, TypeSymbol operandType, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Unary operator " + opToken.text + " is not defined for type " + operandType + ".", where, DateTime.Now);
        }

        internal static void BinderError_BinaryOperatorNotDefined(this Diagnostics diagnostics, LanguageToken opToken, TypeSymbol leftType, TypeSymbol rightType, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Binary operator " + opToken.text + " is not defined for types " + leftType + ", " + rightType + ".", where, DateTime.Now);
        }

        internal static void BinderError_VariableDoesNotExist(this Diagnostics diagnostics, string varName, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "No variable declared with name " + varName + ".", where, DateTime.Now);
        }

        internal static void BinderError_VariableAlreadyDeclared(this Diagnostics diagnostics, VariableSymbol var, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Variable " + var.name + " (" + var.type + ") has already been declared in this scope.", where, DateTime.Now);
        }

        internal static void BinderError_CannotCastVariable(this Diagnostics diagnostics, VariableSymbol symbol, TypeSymbol type, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Variable " + symbol.name + " (" + symbol.type + ") cannot cast to " + type, where, DateTime.Now);
        }

        internal static void BinderError_VariableUndeclared(this Diagnostics diagnostics, string name, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Variable " + name + " is not previously defined.", where, DateTime.Now);
        }

        internal static void BinderError_ReadonlyVariableAssignment(this Diagnostics diagnostics, VariableSymbol symbol, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Variable " + symbol.name + " (" + symbol.type + ") is readonly and cannot be assigned to.", where, DateTime.Now);
        }

        internal static void BinderError_CannotConvertExpressionType(this Diagnostics diagnostics, TypeSymbol target, TypeSymbol actual, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Could not convert expression result from " + actual + " to " + target, where, DateTime.Now);
        }

        internal static void BinderError_UndefinedFunction(this Diagnostics diagnostics, string identifier, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Function '" + identifier + "' is undefined in current scope.", where, DateTime.Now);
        }

        internal static void BinderError_MismatchedArgumentCount(this Diagnostics diagnostics, string identifier, int actual, int expected, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Function argument count mismatch for '" + identifier + "'. Expected " + expected + ", got " + actual, where, DateTime.Now);
        }

        internal static void BinderError_ArgumentTypeMismatch(this Diagnostics diagnostics, TypeSymbol actual, TypeSymbol expected, int index, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Argument " + index + " was expected to be " + expected + ", got " + actual, where, DateTime.Now);
        }

        internal static void BinderError_ExpressionMustReturnValue(this Diagnostics diagnostics, TextSpan where)
        {
            diagnostics.AddFailure("Binder", "Expression must return a value (not void).", where, DateTime.Now);
        }

        internal static void EvaluatorError_UnexpectedUnaryOperator(this Diagnostics diagnostics, BoundUnaryOperator op, TextSpan where)
        {
            diagnostics.AddFailure("Evaluator", "Unexpected unary operator in tree " + op, where, DateTime.Now);
        }

        internal static void EvaluatorError_UnxpectedBinaryOperator(this Diagnostics diagnostics, BoundBinaryOperator op, TextSpan where)
        {
            diagnostics.AddFailure("Evaluator", "Unexpected binary operator in tree " + op, where, DateTime.Now);
        }
    }
}