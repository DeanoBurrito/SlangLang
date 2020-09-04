using System;
using System.Collections.Generic;
using System.Reflection;
using SlangLang.Debug;

namespace SlangLang.Parsing
{
    public abstract class ExpressionNode : ParseNode
    {
        public readonly LanguageToken token;

        public ExpressionNode(LanguageToken token, ParseNodeType type, TextSpan where) : base(type, where)
        {
            this.token = token;
        }

        public List<ExpressionNode> GetChildren()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<ExpressionNode> nodeList = new List<ExpressionNode>();
            foreach (FieldInfo info in fields)
            {
                if (typeof(ExpressionNode).IsAssignableFrom(info.FieldType))
                {
                    ExpressionNode child = (ExpressionNode)info.GetValue(this);
                    nodeList.Add(child);
                }
                else if (typeof(List<ExpressionNode>).IsAssignableFrom(info.FieldType))
                {
                    List<ExpressionNode> children = (List<ExpressionNode>)info.GetValue(this);
                    foreach (ExpressionNode child in children)
                        nodeList.Add(child);
                }
            }
            return nodeList;
        }

        public abstract override string ToString();
    }
}