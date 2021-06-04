using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Test_Parser;

namespace GrandTextAdventure.Core.Parsing.Tokenizer
{
    public class TokenDefinition
    {
        private readonly int _precedence;
        private readonly Regex _regex;
        private readonly SyntaxKind _returnsToken;
        private readonly Type _returnType;

        public TokenDefinition(SyntaxKind returnsToken, string regexPattern, int precedence, Type returnType = null)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _returnsToken = returnsToken;
            _precedence = precedence;
            _returnType = returnType;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = _regex.Matches(inputString);
            for (var i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch()
                {
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    TokenType = _returnsToken,
                    Value = matches[i].Value,
                    Precedence = _precedence,
                    ReturnType = _returnType,
                };
            }
        }
    }
}