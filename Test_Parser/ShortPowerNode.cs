using GrandTextAdventure.Core.Parsing;

namespace Test_Parser
{
    internal class ShortPowerNode : SyntaxNode
    {
        private SyntaxNode Base;
        private SyntaxNode Power;

        public ShortPowerNode(SyntaxNode atom, SyntaxNode power, bool IsNegative)
        {
            this.Base1 = atom;
            this.Power1 = power;
            this.IsNegative = IsNegative;
        }

        public SyntaxNode Base1 { get => Base; set => Base = value; }
        public bool IsNegative { get; }
        public SyntaxNode Power1 { get => Power; set => Power = value; }

        public override void Accept(IScriptVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}