using GrandTextAdventure.Core.Parsing;
using System;
using System.Collections.Generic;

namespace Test_Parser
{
    public enum Operator
    {
        Addition,
        Subtraktion,
        Multiplication,
        Division,
        Power,
        Modulo
    }

    public class AbsoluteValueExpression : SyntaxNode
    {
        public SyntaxNode Expression { get; set; }

        public override void Accept(IScriptVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class BinaryExpression : SyntaxNode
    {
        public SyntaxNode Left { get; set; }
        public Operator Op { get; set; }
        public SyntaxNode Right { get; set; }

        public override void Accept(IScriptVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class FuncCallExpression : SyntaxNode
    {
        public FuncCallExpression(string name, SyntaxNode args)
        {
            Name = name;
            Args = args;
        }

        public SyntaxNode Args { get; set; }

        public string Name { get; set; }

        public override void Accept(IScriptVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class FunctionDefinitionNode : SyntaxNode
    {
        public List<string> Args { get; set; } = new();
        public SyntaxNode Body { get; set; }
        public string Name { get; set; }

        public override void Accept(IScriptVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class GroupExpression : SyntaxNode
    {
        public SyntaxNode Expression { get; set; }
        public bool IsNegated { get; internal set; }

        public override void Accept(IScriptVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class Number : SyntaxNode
    {
        public Number(double value)
        {
            Value = value;
        }

        public double Value { get; set; }

        public override void Accept(IScriptVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}