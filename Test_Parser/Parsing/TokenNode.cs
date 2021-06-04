using GrandTextAdventure.Core.Parsing.Text;
using GrandTextAdventure.Core.Parsing.Tokenizer;

namespace GrandTextAdventure.Core.Parsing
{
    public class TokenNode : SyntaxNode
    {
        public TokenNode(Token token)
        {
            Token = token;
        }

        public override TextSpan Span => Token.Span;
        public Token Token { get; set; }

        public override void Accept(IScriptVisitor visitor)
        {
        }
    }
}