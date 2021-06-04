using GrandTextAdventure.Core.Parsing;
using GrandTextAdventure.Core.Parsing.Tokenizer;

namespace Test_Parser
{
    internal class NameExpression : SyntaxNode
    {
        private readonly Token identifierToken;

        public NameExpression(Token identifierToken)
        {
            this.identifierToken = identifierToken;
        }

        public string Identifier => identifierToken.Text;

        public override void Accept(IScriptVisitor visitor)
        {
        }
    }
}