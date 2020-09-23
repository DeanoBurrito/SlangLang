using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SlangLang.Parsing
{
    public sealed class SeparatedNodeList<T> : IEnumerable<T>
        where T: ParseNode
    {
        public readonly ImmutableArray<ParseNode> separatorsAndNodes;
        public readonly int count;
        
        public SeparatedNodeList(ImmutableArray<ParseNode> separatorsAndNodes)
        {
            this.separatorsAndNodes = separatorsAndNodes;
            count = (separatorsAndNodes.Length + 1) / 2;
        }

        public T this[int index]
        {
            get
            {
                return (T)separatorsAndNodes[index * 2];
            }
        }

        public ParseNode GetSeparator(int index)
        {
            if (index == count - 1)
                return null;
            return (ParseNode)separatorsAndNodes[index * 2 + 1];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}