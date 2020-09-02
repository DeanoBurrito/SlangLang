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
        [Fact]
        public void VerifyAllTokensTested()
        {
            List<LanguageTokenType> allTokens = Enum.GetValues(typeof(LanguageTokenType)).Cast<LanguageTokenType>().ToList();
            List<LanguageTokenType> testedTokens = GetTokens().Concat(GetSeparators()).Select(t => t.Item1).ToList();
            SortedSet<LanguageTokenType> untested = new SortedSet<LanguageTokenType>(allTokens);
            untested.Remove(LanguageTokenType.BadToken);
            untested.Remove(LanguageTokenType.EndOfFile);
            untested.ExceptWith(testedTokens);

            Assert.Empty(untested);
        }
        
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void LexTokenSingle(LanguageTokenType tokenType, string text)
        {
            Lexer lexer = new Lexer(Diagnostics.DummyInstance, new string[] { text }, "Tests");
            LanguageToken[] tokens = lexer.LexAll();
            
            Assert.Equal(tokens.Last().tokenType, LanguageTokenType.EndOfFile);
            Array.Resize(ref tokens, tokens.Length - 1);

            LanguageToken token = Assert.Single(tokens);
            Assert.Equal(tokenType, token.tokenType);
            Assert.Equal(token.text, text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void LexTokenPairs(LanguageTokenType type1, string text1, LanguageTokenType type2, string text2)
        {
            string fullText = text1 + text2;
            TextStore textStore = new TextStore("Tests", new string[] { fullText });
            Lexer lexer = new Lexer(Diagnostics.DummyInstance, new string[] { text1 + text2 }, "Tests");
            LanguageToken[] tokens = lexer.LexAll();

            Assert.Equal(tokens.Last().tokenType, LanguageTokenType.EndOfFile);
            Array.Resize(ref tokens, tokens.Length - 1);

            Assert.Equal(tokens.Length, 2);
            Assert.Equal(tokens[0].tokenType, type1);
            Assert.Equal(tokens[0].text, text1);
            Assert.Equal(tokens[1].tokenType, type2);
            Assert.Equal(tokens[1].text, text2);

            Assert.Equal(text1, textStore.GetSubstring(tokens[0].textLocation));
            Assert.Equal(text2, textStore.GetSubstring(tokens[1].textLocation));
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSeparatorsData))]
        public void LexTokenPairsWithSeparators(LanguageTokenType l1, string t1, LanguageTokenType ls, string ts, LanguageTokenType l2, string t2)
        {
            Lexer lexer = new Lexer(Diagnostics.DummyInstance, new string[] { t1 + ts + t2 }, "Tests");
            LanguageToken[] tokens = lexer.LexAll();

            Assert.Equal(tokens.Last().tokenType, LanguageTokenType.EndOfFile);
            Array.Resize(ref tokens, tokens.Length - 1);

            Assert.Equal(tokens.Length, 3);
            Assert.Equal(tokens[0].tokenType, l1);
            Assert.Equal(tokens[0].text, t1);
            Assert.Equal(tokens[1].tokenType, ls);
            Assert.Equal(tokens[1].text, ts);
            Assert.Equal(tokens[2].tokenType, l2);
            Assert.Equal(tokens[2].text, t2);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach ((LanguageTokenType tokenType, string t) pair in GetTokens())
            {
                yield return new object[] { pair.tokenType, pair.t };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach ((LanguageTokenType type1, string text1, LanguageTokenType type2, string text2) pair in GetTokenPairs())
            {
                yield return new object[] { pair.type1, pair.text1, pair.type2, pair.text2 };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsWithSeparatorsData()
        {
            foreach ((LanguageTokenType type1, string text1,LanguageTokenType sepType, string sepText, LanguageTokenType type2, string text2) item in GetTokenPairsWithSeparators())
            {
                yield return new object[] { item.type1, item.text1, item.sepType, item.sepText, item.type2, item.text2 };
            }
        }

        private static IEnumerable<(LanguageTokenType tokenType, string text)> GetTokens()
        {
            IEnumerable<(LanguageTokenType, string)> fixedTokens = Enum.GetValues(typeof(LanguageTokenType))
                .Cast<LanguageTokenType>()
                .Select(t => (type: t, text: LanguageFacts.GetText(t)))
                .Where(t => t.text != null);
            
            (LanguageTokenType, string)[] dynamicTokens = 
            {
                (LanguageTokenType.IntegerNumber, "1"),
                (LanguageTokenType.IntegerNumber, "10"),
                (LanguageTokenType.IntegerNumber, "010"),
                (LanguageTokenType.String, "\"This is a string literal.\""),
                (LanguageTokenType.Identifier, "a"),
                (LanguageTokenType.Identifier, "abc"),
                (LanguageTokenType.Identifier, "A"),
            };
            return fixedTokens.Concat(dynamicTokens);
        }

        private static IEnumerable<(LanguageTokenType type, string text)> GetSeparators()
        {
            yield return (LanguageTokenType.Whitespace, " ");
            yield return (LanguageTokenType.Whitespace, "  ");
            yield return (LanguageTokenType.Whitespace, "\r");
            yield return (LanguageTokenType.Whitespace, "\n");
            yield return (LanguageTokenType.Whitespace, "\r\n");
        }

        private static bool TokensRequiresSeparator(LanguageTokenType type1, LanguageTokenType type2)
        {
            //what a hack. Wow.
            bool is1Keyword = type1.ToString().StartsWith("Keyword");
            bool is2Keyword = type2.ToString().StartsWith("Keyword");

            if (type1 == LanguageTokenType.Identifier && type2 == LanguageTokenType.Identifier)
                return true;

            if (is1Keyword && is2Keyword)
                return true;

            if ((is1Keyword && type2 == LanguageTokenType.Identifier) ||
                (is2Keyword && type1 == LanguageTokenType.Identifier))
                return true;

            if (type1 == LanguageTokenType.IntegerNumber && type2 == LanguageTokenType.IntegerNumber)
                return true;

            // (& + &), (& + &&), (&& + &)
            if ((type1 == LanguageTokenType.And && type2 == LanguageTokenType.And) ||
                (type1 == LanguageTokenType.And && type2 == LanguageTokenType.AndAnd))
                return true;

            // (| + |), (| + ||), (|| + |)
            if ((type1 == LanguageTokenType.Pipe && type2 == LanguageTokenType.Pipe) ||
                (type1 == LanguageTokenType.Pipe && type2 == LanguageTokenType.PipePipe))
                return true;
            
            // (= + =), (= + ==), (== + =)
            if ((type1 == LanguageTokenType.Equals && type2 == LanguageTokenType.Equals) ||
                (type1 == LanguageTokenType.Equals && type2 == LanguageTokenType.EqualsEquals))
                return true;
            
            if (type1 == LanguageTokenType.Exclamation && (type2 == LanguageTokenType.Equals 
                || type2 == LanguageTokenType.EqualsEquals))
                return true;
            
            return false;
        }

        private static IEnumerable<(LanguageTokenType type1, string text1,LanguageTokenType sepType, string sepText, LanguageTokenType type2, string text2)> GetTokenPairsWithSeparators()
        { //oof, lets go java-style.
            foreach ((LanguageTokenType, string) t1 in GetTokens())
            {
                foreach ((LanguageTokenType, string) t2 in GetTokens())
                {
                    if (t1.Item1 == LanguageTokenType.Whitespace && t2.Item1 == LanguageTokenType.Whitespace)
                        continue;
                    
                    if (TokensRequiresSeparator(t1.Item1, t2.Item1))
                    {
                        foreach ((LanguageTokenType type, string text) sep in GetSeparators())
                            yield return (t1.Item1, t1.Item2, sep.type, sep.text, t2.Item1, t2.Item2);
                    }
                }
            }
        }

        private static IEnumerable<(LanguageTokenType type1, string text1, LanguageTokenType type2, string text2)> GetTokenPairs()
        {
            foreach ((LanguageTokenType, string) t1 in GetTokens())
            {
                foreach ((LanguageTokenType, string) t2 in GetTokens())
                {
                    if (t1.Item1 == LanguageTokenType.Whitespace && t2.Item1 == LanguageTokenType.Whitespace)
                        continue;
                    
                    if (!TokensRequiresSeparator(t1.Item1, t2.Item1))
                        yield return (t1.Item1, t1.Item2, t2.Item1, t2.Item2);
                }
            }
        }
    }
}
