using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GrandTextAdventure.Core.Parsing
{
    public class BlockNode : SyntaxNode, IEnumerable<SyntaxNode>
    {
        public BlockNode(IEnumerable<SyntaxNode> children)
        {
            Children = children;
        }

        public IEnumerable<SyntaxNode> Children { get; }

        public static BlockNode Concat(BlockNode first, BlockNode second)
        {
            return new BlockNode(first.Children.Concat(second.Children));
        }

        public override void Accept(IScriptVisitor visitor)
        {
            //visitor.Visit(this);
        }

        public IEnumerable<T> Descendants<T>()
            where T : SyntaxNode
        {
            foreach (var n in Children)
            {
                if (n is BlockNode nbn)
                {
                    foreach (var nc in nbn.Descendants<T>())
                    {
                        yield return nc;
                    }
                }
                else if (n is T t)
                {
                    yield return t;
                }
            }
        }

        public IEnumerator<SyntaxNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}