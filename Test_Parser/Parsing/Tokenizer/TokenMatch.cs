using System;
using Test_Parser;

namespace GrandTextAdventure.Core.Parsing.Tokenizer
{
    public class TokenMatch
    {
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
        public Type ReturnType { get; internal set; }
        public int StartIndex { get; set; }
        public SyntaxKind TokenType { get; set; }
        public string Value { get; set; }
    }
}