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
        [InlineData("(1 == 1 && 3 != 2) && (true || false);", true)]
        [InlineData("{ int c = 2; (c = 10) * c; }", 100)]
        [InlineData("int c = 33;", 33)]
        public void Tests(string text, object expectedValue)
        {
            Compilation comp = new Compilation(new Debug.TextStore("Tests", new string[] { text }), CompilationOptions.DefaultOptions);
            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>() 
            {
                { new VariableSymbol("a", false, typeof(int)), 10 },
                { new VariableSymbol("b", false, typeof(int)), 41 },
            };
            Evaluation.EvaluationResult result = comp.Evaluate(variables);
            
            Assert.False(result.diagnostics.HasErrors);
            Assert.Equal(expectedValue, result.value);
        }
    }
}