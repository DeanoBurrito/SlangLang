using System;
using System.Collections.Generic;
using Xunit;
using SlangLang.Binding;
using SlangLang.Drivers;

namespace SlangLang.Tests
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1;", 1)]
        [InlineData("-1;", -1)]
        [InlineData("3 + 4;", 7)]
        [InlineData("7 - 5;", 2)]
        [InlineData("2 * 3;", 6)]
        [InlineData("10 / 5;", 2)]
        [InlineData("1 + 2 * 3;", 7)]
        [InlineData("(1 + 2) * 3;", 9)]
        [InlineData("false;", false)]
        [InlineData("true;", true)]
        [InlineData("!true;", false)]
        [InlineData("false == false;", true)]
        [InlineData("true == false;", false)]
        [InlineData("3 == 3;", true)]
        [InlineData("7 != 2;", true)]
        [InlineData("5 < 1;", false)]
        [InlineData("1 <= 3;", true)]
        [InlineData("10 > 4;", true)]
        [InlineData("5 >= 6;", false)]
        [InlineData("(1 == 1 && 3 != 2) && (true || false);", true)]
        [InlineData("{ int c = 2; (c = 10) * c; }", 100)]
        [InlineData("int c = 33;", 33)]
        [InlineData("{ let int a = 1; int b = 0; if a == 1 b = 1; b; }", 1)]
        [InlineData("{ let int a = 5; int b = 0; if a == 1 b = 1; b; }", 0)]
        [InlineData("{ let int a = 1; int b = 0; if a == 1 b = 1; else b = -1; b; } ", 1)]
        [InlineData("{ let int a = 10; int b = 0; if a == 1 b = 1; else b = -1; b; } ", -1)]
        [InlineData("{ int i = 0; int x = 0; while i < 10 { x = x + 2; i = i + 1; } x;}", 20)]
        public void Tests(string text, object expectedValue)
        {
            Compilation comp = new Compilation(new Debug.TextStore("Tests", new string[] { text }), CompilationOptions.DefaultOptions);
            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
            Evaluation.EvaluationResult result = comp.Evaluate(variables);
            
            Assert.False(result.diagnostics.HasErrors);
            Assert.Equal(expectedValue, result.value);
        }
    }
}