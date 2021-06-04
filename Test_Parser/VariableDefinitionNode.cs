using GrandTextAdventure.Core.Parsing;

namespace Test_Parser
{
    internal class VariableDefinitionNode : SyntaxNode
    {
        public VariableDefinitionNode(string name, SyntaxNode expr)
        {
            Name = name;
            Expr = expr;
        }

        public SyntaxNode Expr { get; }
        public string Name { get; }

        public override void Accept(IScriptVisitor visitor)
        {
        }
    }
}