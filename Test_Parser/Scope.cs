using GrandTextAdventure.Core.Parsing;
using System;
using System.Collections.Generic;

namespace Test_Parser
{
    public class Scope
    {
        private Dictionary<string, SyntaxNode> _variables = new();

        public Scope Parent { get; set; }

        public void Add(string name, SyntaxNode value)
        {
            if (_variables.ContainsKey(name))
            {
                _variables[name] = value;
            }
            else
            {
                _variables.Add(name, value);
            }
        }

        public Scope CreateScope()
        {
            var s = new Scope();
            s.Parent = this;

            return s;
        }

        public SyntaxNode GetValue(string name)
        {
            if (_variables.ContainsKey(name))
            {
                return _variables[name];
            }
            else
            {
                if (Parent != null)
                {
                    return Parent.GetValue(name);
                }
            }

            Console.WriteLine($"Variable '{name}' is not defined");

            return new Number(0);
        }
    }
}