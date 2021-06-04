using GrandTextAdventure.Core.Parsing;
using System.Collections.Generic;

namespace Test_Parser
{
    public class Function
    {
        public SyntaxNode Body { get; set; }
        public List<string> Parameters { get; internal set; } = new();

        public double Invoke(Scope scope)
        {
            return Evaluator.Evaluate(Body, scope);
        }
    }
}