using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SlangLang.Parsing;
using SlangLang.Debug;

namespace SlangLang.Tests
{
    public class LexerTests
    {

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void VerifyLexing(LanguageTokenType tokenType, string text)
        {
            Lexer lexer = new Lexer(new Diagnostics(DateTime.Now), text, "Tests");
            LanguageToken[] tokens = lexer.LexAll();
            
            Assert.Equal(tokens.Last().tokenType, LanguageTokenType.EndOfFile);
            Array.Resize(ref tokens, tokens.Length - 1);
            LanguageToken token = Assert.Single(tokens);
            Assert.Equal(tokenType, token.tokenType);
            Assert.Equal(token.text, text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach ((LanguageTokenType tokenType, string t) pair in GetTokens())
            {
                yield return new object[] { pair.tokenType, pair.t };
            }
        }

        private static IEnumerable<(LanguageTokenType tokenType, string text)> GetTokens()
        {
            yield return (LanguageTokenType.Identifier, "a");
            yield return (LanguageTokenType.Identifier, "aa");
            yield return (LanguageTokenType.Identifier, "abc");
            yield return (LanguageTokenType.Identifier, "bac");

            yield return (LanguageTokenType.Equals, "=");
            yield return (LanguageTokenType.EqualsEquals, "==");
            yield return (LanguageTokenType.Exclamation, "!");
            yield return (LanguageTokenType.ExclamationEquals, "!=");
        }
    }
}
