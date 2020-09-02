using System;
using System.Collections.Generic;
using System.Reflection;
using SlangLang.Debug;

namespace SlangLang.Binding
{
    internal abstract class BoundExpression
    {
        public readonly Type boundType;
        public readonly BoundNodeType nodeType;
        public readonly TextSpan textLocation;
        
        public BoundExpression(Type bindingType, BoundNodeType nodeType, TextSpan where)
        {
            boundType = bindingType;
            this.nodeType = nodeType;
            textLocation = where;
        }

        public List<BoundExpression> GetChildren()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<BoundExpression> nodeList = new List<BoundExpression>();
            foreach (FieldInfo info in fields)
            {
                if (typeof(BoundExpression).IsAssignableFrom(info.FieldType))
                {
                    BoundExpression child = (BoundExpression)info.GetValue(this);
                    nodeList.Add(child);
                }
                else if (typeof(List<BoundExpression>).IsAssignableFrom(info.FieldType))
                {
                    List<BoundExpression> children = (List<BoundExpression>)info.GetValue(this);
                    foreach (BoundExpression child in children)
                        nodeList.Add(child);
                }
            }
            return nodeList;
        }

        public abstract override string ToString();
    }
}