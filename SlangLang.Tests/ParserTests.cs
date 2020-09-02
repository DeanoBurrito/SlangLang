using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SlangLang.Parsing;

namespace SlangLang.Tests
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void BinaryExpressionHonoursPrecedence(LanguageTokenType op1, LanguageTokenType op2)
        {
            int op1Precedence = LanguageFacts.GetBinaryOperatorPrecedence(new LanguageToken(op1, "", null));
            int op2Precedence = LanguageFacts.GetBinaryOperatorPrecedence(new LanguageToken(op2, "", null));
            string text = $"a {LanguageFacts.GetText(op1)} b {LanguageFacts.GetText(op2)} c";

            Debug.Diagnostics diagnostics = new Debug.Diagnostics(DateTime.Now);
            ExpressionNode expression = new Parser(diagnostics, new Lexer(diagnostics, new string[] { text }, "Tests").LexAll()).ParseAll();
            Assert.False(diagnostics.HasErrors);

            if (op1Precedence >= op2Precedence)
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNode(ExpressionNodeType.Binary);
                        e.AssertNode(ExpressionNodeType.Binary);
                            e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "a");
                            e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "b");
                        e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "c");
                }
            }
            else
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNode(ExpressionNodeType.Binary);
                        e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "a");
                        e.AssertNode(ExpressionNodeType.Binary);
                            e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "b");
                            e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "c");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void UnaryExpressionHonoursPrecedence(LanguageTokenType unaryOp, LanguageTokenType BinaryOp)
        {
            int unaryPrecedence = LanguageFacts.GetUnaryOperatorPrecedence(new LanguageToken(unaryOp, "", null));
            int binaryPrecedence = LanguageFacts.GetUnaryOperatorPrecedence(new LanguageToken(BinaryOp, "", null));
            string text = $"{LanguageFacts.GetText(unaryOp)} a {LanguageFacts.GetText(BinaryOp)} b";
            
            Debug.Diagnostics diagnostics = new Debug.Diagnostics(DateTime.Now);
            ExpressionNode expression = new Parser(diagnostics, new Lexer(diagnostics, new string[] { text }, "Tests").LexAll()).ParseAll();
            Assert.False(diagnostics.HasErrors);

            if (unaryPrecedence >= binaryPrecedence)
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNode(ExpressionNodeType.Binary);
                        e.AssertNodeAndToken(ExpressionNodeType.Unary, unaryOp, LanguageFacts.GetText(unaryOp));
                            e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "a");
                        e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "b");
                }
            }
            else
            {
                using (AssertingExpressionTree e = new AssertingExpressionTree(expression))
                {
                    e.AssertNodeAndToken(ExpressionNodeType.Unary, unaryOp, LanguageFacts.GetText(unaryOp));
                        e.AssertNode(ExpressionNodeType.Binary);
                            e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "a");
                            e.AssertNodeAndToken(ExpressionNodeType.Name, LanguageTokenType.Identifier, "b");
                }
            }
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
