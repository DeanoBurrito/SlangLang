using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SlangLang.Parsing;

namespace SlangLang.Tests
{
    internal sealed class AssertingExpressionTree : IDisposable
    {
        private readonly IEnumerator<ParseNode> enumerator;
        
        public AssertingExpressionTree(ParseNode node)
        {
            enumerator = Flattern(node).GetEnumerator();
        }

        private static IEnumerable<ParseNode> Flattern(ParseNode node)
        {
            Stack<ParseNode> stack = new Stack<ParseNode>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                ParseNode popped = stack.Pop();
                yield return popped;

                List<ParseNode> children = popped.GetChildren();
                children.Reverse();
                foreach (ParseNode child in children)
                {
                    stack.Push(child);
                }
            }
        }

        public void AssertNodeAndToken(ParseNodeType nodeType, LanguageTokenType type, string text)
        {
            Assert.True(enumerator.MoveNext());
            Assert.Equal(nodeType, enumerator.Current.nodeType);
            
            ExpressionNode node = Assert.IsType<ExpressionNode>(enumerator.Current);
            LanguageToken token = Assert.IsType<LanguageToken>(node.token);
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