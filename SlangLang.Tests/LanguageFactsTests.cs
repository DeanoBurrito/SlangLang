using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SlangLang.Parsing;

namespace SlangLang.Tests
{
    public class LanguageFactsTests
    {
        [Theory]
        [MemberData(nameof(GetRoundtripData))]
        public void GetTextRoundtrip(LanguageTokenType tokenType)
        {
            string text = LanguageFacts.GetText(tokenType);
            if (text == null)
                return;
            
            LanguageToken[] tokens = new Lexer(new Debug.Diagnostics(DateTime.Now), new string[] { text }, "Tests").LexAll();
            Assert.Equal(tokens.Last().tokenType, LanguageTokenType.EndOfFile);
            Array.Resize(ref tokens, tokens.Length - 1);

            LanguageToken token = Assert.Single(tokens);
            Assert.Equal(token.tokenType, tokenType);
            Assert.Equal(token.text, text);
        }

        public static IEnumerable<object[]> GetRoundtripData()
        {
            LanguageTokenType[] types = (LanguageTokenType[])Enum.GetValues(typeof(LanguageTokenType));
            foreach (LanguageTokenType l in types)
                yield return new object[] { l };
        }
    }
}