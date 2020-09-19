using System;
using System.Collections.Generic;
using Xunit;
using SlangLang.Symbols;
using SlangLang.Drivers;
using SlangLang.Debug;

namespace SlangLang.Tests
{
    public class EvaluatorTests
    {
        [Theory]
        [MemberData(nameof(GetFullTestData))]
        public void Tests(string text, object expectedValue)
        {
            Compilation comp = new Compilation(new Debug.TextStore("Tests", new string[] { text }), CompilationOptions.DefaultOptions);
            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
            Evaluation.EvaluationResult result = comp.Evaluate(variables);
            
            Assert.False(result.diagnostics.HasErrors);
            Assert.Equal(expectedValue, result.value);
        }

        public static IEnumerable<object[]> GetFullTestData()
        {
            yield return new object[] { "1;", 1 };
            yield return new object[] { "-1;", -1 };

            yield return new object[] { "3 + 4;", 7 };
            yield return new object[] { "7 - 5;", 2 };
            yield return new object[] { "2 * 3;", 6 };
            yield return new object[] { "10 / 5;", 2 };
            yield return new object[] { "1 + 2 * 3;", 7 };
            yield return new object[] { "(1 + 2) * 3;", 9 };

            yield return new object[] { "false;", false };
            yield return new object[] { "true;", true };
            yield return new object[] { "!true;", false };
            yield return new object[] { "false == false;", true };
            yield return new object[] { "true == false;", false };
            yield return new object[] { "true == true;", true};

            yield return new object[] { "false | false;", false };
            yield return new object[] { "false | true;", true };
            yield return new object[] { "true | true;", true };
            yield return new object[] { "false & false;", false };
            yield return new object[] { "false & true;", false };
            yield return new object[] { "true & true;", true };
            yield return new object[] { "true ^ false;", true };
            yield return new object[] { "false ^ false;", false };

            yield return new object[] { "1 | 2;", 3 };
            yield return new object[] { "1 | 0;", 1 };
            yield return new object[] { "2 & 3;", 2 };
            yield return new object[] { "1 & 0;", 0 };
            yield return new object[] { "1 ^ 0;", 1 };
            yield return new object[] { "1 ^ 1;", 0 };
            yield return new object[] { "1 ^ 3;" , 2 };

            yield return new object[] { "3 == 3;", true };
            yield return new object[] { "7 != 2;", true };
            yield return new object[] { "5 < 1;", false };
            yield return new object[] { "1 <= 3;", true };
            yield return new object[] { "10 > 4;", true };
            yield return new object[] { "5 >= 6;", false };
            yield return new object[] { "(1 == 1 && 3 != 2) && (true || false);", true };

            yield return new object[] { "{ int c = 2; (c = 10) * c; }", 100 };
            yield return new object[] { "int c = 33;", 33 };

            yield return new object[] { "{ let int a = 1; int b = 0; if a == 1 b = 1; b; }", 1 };
            yield return new object[] { "{ let int a = 5; int b = 0; if a == 1 b = 1; b; }", 0 };
            yield return new object[] { "{ let int a = 1; int b = 0; if a == 1 b = 1; else b = -1; b; } ", 1 };
            yield return new object[] { "{ let int a = 10; int b = 0; if a == 1 b = 1; else b = -1; b; } ", -1 };

            yield return new object[] { "{ int i = 0; int x = 0; while i < 10 { x = x + 2; i = i + 1; } x;}", 20 };

            yield return new object[] { "{ int a = 0; for int i = 0; i < 5; i = i + 1; a = a + 2; a;}", 10 };
        }

        [Fact]
        public void ErrorWriteReadonlyVariable()
        {
            string src = @"
            {
                let int a = 0;
                [a = 10];
            }
            ";
            string error = @"
            Variable a (Int32) is readonly and cannot be assigned to.
            ";
            AssertDiagnostics(src, error);
        }

        [Fact]
        public void ErrorBadInput()
        {
            string src = @"
            {
                int a = 0; 
                while (a != 0)
                    int b = 0;
                [$]
            }
            ";
            string error = @"
            Bad character in input text, found $.
            ";
            AssertDiagnostics(src, error);
        }

        [Fact]
        public void ErrorTokenMatchFailed()
        {
            string src = @"
            {
                int a = [;]
            }
            ";
            string error = @"
            Token match failed, expected Identifier, found Semicolon instead.
            ";
            AssertDiagnostics(src, error);

            src = @"
            {
                int a = (10 * [)];
            }
            ";
            error = @"
            Token match failed, expected Identifier, found CloseParanthesis instead.
            ";
            AssertDiagnostics(src, error);
        }

        [Fact]
        public void ErrorUnaryOperatorNotDefined()
        {
            string src = @"
            {
                [!0];
            }
            ";
            string error = @"
            Unary operator ! is not defined for type Int32.
            ";
            AssertDiagnostics(src, error);
        }

        [Fact]
        public void ErrorBinaryOperatorNotDefined()
        {
            string src = @"
            {
                int c = 10;
                c = 200;
                [10 || 20];
            }
            ";
            string error = @"
            Binary operator || is not defined for types Int32, Int32.
            ";
            AssertDiagnostics(src, error);
        }

        [Fact]
        public void ErrorVariableDoesNotExists()
        {
            string src = @"
            {
                int a = 10;
                [b] = true;
                {
                    let int b = 10;
                    [c] = 10;
                }
            }
            ";
            string error = @"
            Variable b is not previously defined.
            Variable c is not previously defined.
            ";
            AssertDiagnostics(src, error);
        }

        [Fact]
        public void ErrorVariableAlreadyDeclared()
        {
            string src = @"
            {
                int a = 10;
                int z = 20;
                {
                    a = 20;
                    [int z] = 20; 
                }
            }
            ";
            string error = @"
            Variable z (Int32) has already been declared in this scope.
            ";
            AssertDiagnostics(src, error);
        }

        private void AssertDiagnostics(string text, string diagnosticOutput)
        {
            AnnotatedText annotatedText = AnnotatedText.Parse(text);
            string[] processedLines = annotatedText.text.Split(Environment.NewLine);
            for (int i = 0; i < processedLines.Length; i++)
                processedLines[i] = processedLines[i] + Environment.NewLine;
            Compilation compilation = new Compilation(new Debug.TextStore("Tests", processedLines), new CompilationOptions());
            Evaluation.EvaluationResult result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());
            string[] expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticOutput);

            Assert.True(result.diagnostics.HasErrors);
            (string message, TextSpan span)[] collapsedDiagnostics = CollapseDiagnostics(result.diagnostics);

            Assert.Equal(collapsedDiagnostics.Length, expectedDiagnostics.Length);
            Assert.Equal(annotatedText.spans.Length, expectedDiagnostics.Length);

            for (int i = 0; i < expectedDiagnostics.Length; i++)
            {
                string expectedMessage = expectedDiagnostics[i];
                string actualMessage = collapsedDiagnostics[i].message;
                Assert.Equal(expectedMessage, actualMessage);

                TextSpan expectedSpan = annotatedText.spans[i];
                TextSpan actualSpan = collapsedDiagnostics[i].span;
                Assert.Equal(expectedSpan, actualSpan);
            }
        }

        private (string message, TextSpan span)[] CollapseDiagnostics(Diagnostics diagnostics)
        {
            List<(string message, TextSpan span)> lines = new List<(string message, TextSpan span)>();
            foreach (var info in diagnostics.infoEntries)
            {
                foreach (DiagnosticEntry entry in info.Value)
                {
                    lines.Add((entry.message, entry.where));
                }
            }
            foreach (var warning in diagnostics.warningEntries)
            {
                foreach (DiagnosticEntry entry in warning.Value)
                {
                    lines.Add((entry.message, entry.where));
                }
            }
            foreach (var failure in diagnostics.failureEntries)
            {
                foreach (DiagnosticEntry entry in failure.Value)
                {
                    lines.Add((entry.message, entry.where));
                }
            }

            return lines.ToArray();
        }
    }
}