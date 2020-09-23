using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SlangLang.Parsing;
using SlangLang.Debug;

namespace SlangLang.Tests
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void BinaryExpressionHonoursPrecedence(LanguageTokenType op1, LanguageTokenType op2)
        {
            int op1Precedence = LanguageFacts.GetBinaryOperatorPrecedence(new LanguageToken(op1, "", "", null));
            int op2Precedence = LanguageFacts.GetBinaryOperatorPrecedence(new LanguageToken(op2, "", "", null));
            string text = $"a {LanguageFacts.GetText(op1)} b {LanguageFacts.GetText(op2)} c;";

            Debug.Diagnostics diagnostics = new Debug.Diagnostics(DateTime.Now);
            ExpressionNode expression = ParseExpression(text, diagnostics);
            Assert.False(diagnostics.HasErrors);

            if (op1Precedence >= op2Precedence)
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNode(ParseNodeType.BinaryExpression);
                        e.AssertNode(ParseNodeType.BinaryExpression);
                            e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "a");
                            e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "b");
                        e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "c");
                }
            }
            else
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNode(ParseNodeType.BinaryExpression);
                        e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "a");
                        e.AssertNode(ParseNodeType.BinaryExpression);
                            e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "b");
                            e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "c");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void UnaryExpressionHonoursPrecedence(LanguageTokenType unaryOp, LanguageTokenType BinaryOp)
        {
            int unaryPrecedence = LanguageFacts.GetUnaryOperatorPrecedence(new LanguageToken(unaryOp,"", "", null));
            int binaryPrecedence = LanguageFacts.GetUnaryOperatorPrecedence(new LanguageToken(BinaryOp, "", "", null));
            string text = $"{LanguageFacts.GetText(unaryOp)} a {LanguageFacts.GetText(BinaryOp)} b;";
            
            Debug.Diagnostics diagnostics = new Debug.Diagnostics(DateTime.Now);
            ExpressionNode expression = ParseExpression(text, diagnostics);
            Assert.False(diagnostics.HasErrors);

            if (unaryPrecedence >= binaryPrecedence)
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNode(ParseNodeType.BinaryExpression);
                        e.AssertNodeAndToken(ParseNodeType.UnaryExpression, unaryOp, LanguageFacts.GetText(unaryOp));
                            e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "a");
                        e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "b");
                }
            }
            else
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNodeAndToken(ParseNodeType.UnaryExpression, unaryOp, LanguageFacts.GetText(unaryOp));
                        e.AssertNode(ParseNodeType.BinaryExpression);
                            e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "a");
                            e.AssertNodeAndToken(ParseNodeType.NameExpression, LanguageTokenType.Identifier, "b");
                }
            }
        }

        public static ExpressionNode ParseExpression(string text, Diagnostics diag)
        {
            Parser parser = new Parser(diag, new TextStore("Tests", new string[] { text }));
            CompilationUnit cunit = parser.ParseCompilationUnit();
            return Assert.IsType<ExpressionStatement>(cunit.statement).expression;
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (LanguageTokenType op1 in LanguageFacts.GetBinaryOperators())
            {
                foreach (LanguageTokenType op2 in LanguageFacts.GetBinaryOperators())
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach (LanguageTokenType op1 in LanguageFacts.GetUnaryOperators())
            {
                foreach (LanguageTokenType op2 in LanguageFacts.GetBinaryOperators())
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }
    }
}
