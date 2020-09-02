using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SlangLang.Parsing;
using SlangLang.Drivers;

namespace SlangLang.Tests
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("3 + 4", 7)]
        [InlineData("7 - 5", 2)]
        [InlineData("2 * 3", 6)]
        [InlineData("10 / 5", 2)]
        [InlineData("1 + 2 * 3", 7)]
        [InlineData("(1 + 2) * 3", 9)]
        [InlineData("false", false)]
        [InlineData("true", true)]
        [InlineData("!true", false)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]
        [InlineData("3 == 3", true)]
        [InlineData("7 != 2", true)]
        [InlineData("(1 == 1 && 3 != 2) && (true || false)", true)]
        [InlineData("a * a", 100)]
        [InlineData("(c = 10) * c", 100)]
        [InlineData("c = 33", 33)]
        public void Tests(string text, object expectedValue)
        {
            Compilation comp = new Compilation(text, CompilationOptions.DefaultOptions);
            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>() 
            {
                { new VariableSymbol("a", typeof(int)), 10 },
                { new VariableSymbol("b", typeof(int)), 41 },
            };
            Evaluation.EvaluationResult result = comp.Evaluate(variables);
            
            Assert.False(result.diagnostics.HasErrors);
            Assert.Equal(expectedValue, result.value);
        }
    }
}