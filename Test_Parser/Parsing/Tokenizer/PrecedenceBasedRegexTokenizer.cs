using System;
using System.Collections.Generic;
using System.Linq;
using Test_Parser;

namespace GrandTextAdventure.Core.Parsing.Tokenizer
{
    public class PrecedenceBasedRegexTokenizer
    {
        private readonly List<TokenDefinition> _tokenDefinitions;

        public PrecedenceBasedRegexTokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();
        }

        public void AddDefinition(SyntaxKind kind, string pattern, int precedence = 1, Type returnType = null)
        {
            _tokenDefinitions.Add(new TokenDefinition(kind, pattern, precedence, returnType));
        }

        public IEnumerable<Token> Tokenize(string src)
        {
            var tokenMatches = FindTokenMatches(src);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (var i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new Token(bestMatch.TokenType, bestMatch.Value, bestMatch.StartIndex, bestMatch.EndIndex, bestMatch.ReturnType);

                lastMatch = bestMatch;
            }

            yield return new Token(SyntaxKind.EndOfFile, string.Empty, 0, 0, null);
        }

        private List<TokenMatch> FindTokenMatches(string src)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(src).ToList());

            return tokenMatches;
        }
    }
}