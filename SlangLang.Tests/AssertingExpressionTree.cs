using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SlangLang.Parsing;

namespace SlangLang.Tests
{
    internal sealed class AssertingExpressionTree : IDisposable
    {
        private readonly IEnumerator<ExpressionNode> enumerator;
        
        public AssertingExpressionTree(ExpressionNode node)
        {
            enumerator = Flattern(node).GetEnumerator();
        }

        private static IEnumerable<ExpressionNode> Flattern(ExpressionNode node)
        {
            Stack<ExpressionNode> stack = new Stack<ExpressionNode>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                ExpressionNode popped = stack.Pop();
                yield return popped;

                List<ExpressionNode> children = popped.GetChildren();
                children.Reverse();
                foreach (ExpressionNode child in children)
                {
                    stack.Push(child);
                }
            }
        }

        public void AssertNodeAndToken(ParseNodeType nodeType, LanguageTokenType type, string text)
        {
            Assert.True(enumerator.MoveNext());
            Assert.Equal(nodeType, enumerator.Current.nodeType);
            
            LanguageToken token = Assert.IsType<LanguageToken>(enumerator.Current.token);
            Assert.Equal(type, token.tokenType);
            Assert.Equal(text, token.text);
        }

        public void AssertNode(ParseNodeType type)
        {
            Assert.True(enumerator.MoveNext());
            Assert.Equal(type, enumerator.Current.nodeType);
        }

        public void Dispose()
        {
            Assert.False(enumerator.MoveNext());
            enumerator.Dispose();
        }
    }
}