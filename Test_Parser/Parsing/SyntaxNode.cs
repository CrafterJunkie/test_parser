using GrandTextAdventure.Core.Parsing.Text;
using GrandTextAdventure.Core.Parsing.Tokenizer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GrandTextAdventure.Core.Parsing
{
    public abstract class SyntaxNode
    {
        public virtual TextSpan Span
        {
            get
            {
                var children = GetChildren();
                if (children.Any())
                {
                    var first = children.First().Span;
                    var last = children.Last().Span;

                    return TextSpan.FromBounds(first.Start, last.End);
                }

                return TextSpan.FromBounds(0, 0);
            }
        }

        public abstract void Accept(IScriptVisitor visitor);

        public IEnumerable<SyntaxNode> GetChildren()
        {
            var result = new List<SyntaxNode>();

            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this);
                    if (child != null)
                        result.Add(child);
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            if (child != null)
                                result.Add(child);
                        }
                    }
                }
                else if (typeof(BlockNode).IsAssignableFrom(property.PropertyType))
                {
                    var children = (BlockNode)property.GetValue(this);
                    foreach (var child in children.Children)
                    {
                        if (child != null)
                            result.Add(child);
                    }
                }
                else if (typeof(Token).IsAssignableFrom(property.PropertyType))
                {
                    var child = (Token)property.GetValue(this);

                    result.Add(new TokenNode(child));
                }
            }

            return result;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}