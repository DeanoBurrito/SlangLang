using System;
using SlangLang.Parsing;
using SlangLang.Binding;

namespace SlangLang.Debug
{
    internal static class DiagnosticsWriteProxy
    {
        internal static void LexerError_ExpectedEndOfStringLiteral(this Diagnostics diagnostics, string foundInstead, TextLocation where)
        {
            diagnostics.AddFailure("Lexer", $"Expected \" to end string literal, found {foundInstead} instead.", where, DateTime.Now);
        }

        internal static void LexerError_GotBadInput(this Diagnostics diagnostics, string found, TextLocation where)
        {
            diagnostics.AddFailure("Lexer", $"Bad character in input text, found {found}" + ".", where, DateTime.Now);
        }

        internal static void ParserError_CouldNotParseInt(this Diagnostics diagnostics, TextLocation where)
        {
            diagnostics.AddFailure("Parser", "Could not parse int from number token.", where, DateTime.Now);
        }

        internal static void ParserError_TokenMatchFailed(this Diagnostics diagnostics, LanguageTokenType expected, LanguageTokenType actual, TextLocation where)
        {
            diagnostics.AddFailure("Parser", "Token match failed, expected " + expected + ", found " + actual + " instead.", where, DateTime.Now);
        }

        internal static void BinderError_UnexpectedExpressionType(this Diagnostics diagnostics, ParseNodeType type, TextLocation where)
        {
            diagnostics.AddFailure("Binder", "Failed to bind expression in tree, unexpected type " + type + ".", where, DateTime.Now);
        }

        internal static void BinderError_UnaryOperatorNotDefined(this Diagnostics diagnostics, LanguageToken opToken, Type operandType, TextLocation where)
        {
            diagnostics.AddFailure("Binder", "Unary operator " + opToken + " is not defined for type " + operandType + ".", where, DateTime.Now);
        }

        internal static void BinderError_BinaryOperatorNotDefined(this Diagnostics diagnostics, LanguageToken opToken, Type leftType, Type rightType, TextLocation where)
        {
            diagnostics.AddFailure("Binder", "Binary operator " + opToken + " is not defined for types " + leftType + ", " + rightType + ".", where, DateTime.Now);
        }

        internal static void BinderError_VariableDoesNotExist(this Diagnostics diagnostics, string varName, TextLocation where)
        {
            diagnostics.AddFailure("Binder", "Unable to bind to variable " + varName + ", no variable with that name has been defined.", where, DateTime.Now);
        }

        internal static void EvaluatorError_UnexpectedUnaryOperator(this Diagnostics diagnostics, BoundUnaryOperator op, TextLocation where)
        {
            diagnostics.AddFailure("Evaluator", "Unexpected unary operator in tree " + op, where, DateTime.Now);
        }

        internal static void EvaluatorError_UnxpectedBinaryOperator(this Diagnostics diagnostics, BoundBinaryOperator op, TextLocation where)
        {
            diagnostics.AddFailure("Evaluator", "Unexpected binary operator in tree " + op, where, DateTime.Now);
        }
    }
}