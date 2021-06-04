using GrandTextAdventure.Core.Parsing;
using GrandTextAdventure.Core.Parsing.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Parser
{
    public class Parser : BaseParser<SyntaxNode, SyntaxKind>
    {
        protected override void InitTokenizer()
        {
            Tokenizer.AddDefinition(SyntaxKind.Number, "-?[0-9]+(\\.[0-9]+)?", 1, typeof(FloatConverter));

            Tokenizer.AddDefinition(SyntaxKind.EqualsToken, "\\=");
            Tokenizer.AddDefinition(SyntaxKind.Plus, "\\+");
            Tokenizer.AddDefinition(SyntaxKind.Minus, "\\-");
            Tokenizer.AddDefinition(SyntaxKind.Star, "\\*");
            Tokenizer.AddDefinition(SyntaxKind.Slash, "\\/");
            Tokenizer.AddDefinition(SyntaxKind.Cirkum, "\\^");
            Tokenizer.AddDefinition(SyntaxKind.Percentage, "\\%");
            Tokenizer.AddDefinition(SyntaxKind.OpenParen, "\\(");
            Tokenizer.AddDefinition(SyntaxKind.CloseParen, "\\)");
            Tokenizer.AddDefinition(SyntaxKind.Pipe, "\\|");
            Tokenizer.AddDefinition(SyntaxKind.Comma, "\\,");
            Tokenizer.AddDefinition(SyntaxKind.ShortPowerLiterals, "[¹²³⁴⁵⁶⁷⁸⁹⁰]+");
            Tokenizer.AddDefinition(SyntaxKind.ShortPowerMinus, "\\⁻");
            Tokenizer.AddDefinition(SyntaxKind.Identifier, "[_a-zA-Z][_a-zA-Z0-9]*");
        }

        protected override SyntaxNode InternalParse()
        {
            var expr = ParseCompilationUnit();
            MatchToken(SyntaxKind.EndOfFile);

            return expr;
        }

        private bool IsFunctionDefinition()
        {
            //x
            //f(x)
            //f(x) = 12

            Token currToken;
            do
            {
                currToken = NextToken();
            } while (currToken.TokenKind<SyntaxKind>() != SyntaxKind.EqualsToken && currToken.TokenKind<SyntaxKind>() != SyntaxKind.EndOfFile);

            var cond = Peek(-1).TokenKind<SyntaxKind>() == SyntaxKind.EqualsToken;
            _position = 0;

            return cond;
        }

        private SyntaxNode ParseAtom()
        {
            if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Number)
            {
                return ParseNumber();
            }
            else if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Identifier)
            {
                if (Peek(1).TokenKind<SyntaxKind>() == SyntaxKind.OpenParen)
                {
                    return ParseFuncCall();
                }

                return ParseIdentifier();
            }

            throw new Exception("Number or Identifier Expected");
        }

        private SyntaxNode ParseCompilationUnit()
        {
            var members = new List<SyntaxNode>();

            while (Current.TokenKind<SyntaxKind>() != SyntaxKind.EndOfFile)
            {
                var startToken = Current;
                var member = ParseMember();

                if (member is BlockNode bn)
                {
                    members.AddRange(bn.Children);
                }
                else
                {
                    if (member != null)
                    {
                        members.Add(member);
                    }
                }

                // If ParseMember() did not consume any tokens,
                // we need to skip the current token and continue
                // in order to avoid an infinite loop.
                //
                // We don't need to report an error, because we'll
                // already tried to parse an expression statement
                // and reported one.
                if (Current == startToken)
                    NextToken();
            }

            return new BlockNode(members);
        }

        private SyntaxNode ParseExpression()
        {
            var left = ParseTerm();
            if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Plus)
            {
                MatchToken(SyntaxKind.Plus);
                var right = ParseExpression();

                return new BinaryExpression { Left = left, Op = Operator.Addition, Right = right };
            }
            else
            {
                if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Minus)
                {
                    MatchToken(SyntaxKind.Minus);
                    var right = ParseExpression();

                    return new BinaryExpression { Left = left, Op = Operator.Subtraktion, Right = right };
                }
            }

            return left;
        }

        private SyntaxNode ParseFactor()
        {
            SyntaxNode node = null;
            if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Minus)
            {
                MatchToken(SyntaxKind.Minus);
                MatchToken(SyntaxKind.OpenParen);
                var expr = ParseExpression();
                MatchToken(SyntaxKind.CloseParen);

                return new GroupExpression { Expression = expr, IsNegated = true };
            }
            else if (Current.TokenKind<SyntaxKind>() == SyntaxKind.OpenParen)
            {
                MatchToken(SyntaxKind.OpenParen);
                var expr = ParseExpression();
                MatchToken(SyntaxKind.CloseParen);

                return new GroupExpression { Expression = expr };
            }
            else if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Pipe)
            {
                MatchToken(SyntaxKind.Pipe);
                var expr = ParseExpression();
                MatchToken(SyntaxKind.Pipe);

                return new AbsoluteValueExpression { Expression = expr };
            }
            else
            {
                node = ParseShortPower();
            }

            return node;
        }

        private SyntaxNode ParseFuncArgs()
        {
            var members = new List<SyntaxNode>();

            while (Current.TokenKind<SyntaxKind>() != SyntaxKind.CloseParen)
            {
                var startToken = Current;
                var member = ParseExpression();

                if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Comma)
                {
                    NextToken();
                }

                if (member is BlockNode bn)
                {
                    members.AddRange(bn.Children);
                }
                else
                {
                    if (member != null)
                    {
                        members.Add(member);
                    }
                }

                // If ParseMember() did not consume any tokens,
                // we need to skip the current token and continue
                // in order to avoid an infinite loop.
                //
                // We don't need to report an error, because we'll
                // already tried to parse an expression statement
                // and reported one.
                if (Current == startToken)
                    NextToken();
            }

            return new BlockNode(members);
        }

        private SyntaxNode ParseFuncCall()
        {
            var name = MatchToken(SyntaxKind.Identifier);

            MatchToken(SyntaxKind.OpenParen);
            var args = ParseFuncArgs();
            MatchToken(SyntaxKind.CloseParen);

            return new FuncCallExpression(name.Text, args);
        }

        private SyntaxNode ParseFuncDef()
        {
            var name = MatchToken(SyntaxKind.Identifier);

            MatchToken(SyntaxKind.OpenParen);
            var args = ParseFuncDefArgs();
            MatchToken(SyntaxKind.CloseParen);
            MatchToken(SyntaxKind.EqualsToken);

            var body = ParseExpression();

            return new FunctionDefinitionNode { Name = name.Text, Body = body, Args = args };
        }

        private List<string> ParseFuncDefArgs()
        {
            var members = new List<string>();

            while (Current.TokenKind<SyntaxKind>() != SyntaxKind.CloseParen)
            {
                var startToken = Current;
                var member = MatchToken(SyntaxKind.Identifier);

                if (Peek(1).TokenKind<SyntaxKind>() == SyntaxKind.Comma || Current.TokenKind<SyntaxKind>() == SyntaxKind.Comma)
                {
                    NextToken();
                }

                members.Add(member.Text);

                // If ParseMember() did not consume any tokens,
                // we need to skip the current token and continue
                // in order to avoid an infinite loop.
                //
                // We don't need to report an error, because we'll
                // already tried to parse an expression statement
                // and reported one.
                if (Current == startToken)
                    NextToken();
            }

            return members;
        }

        private SyntaxNode ParseIdentifier()
        {
            var identifierToken = MatchToken(SyntaxKind.Identifier);

            return new NameExpression(identifierToken);
        }

        private SyntaxNode ParseMember()
        {
            if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Identifier)
            {
                if (Peek(1).TokenKind<SyntaxKind>() == SyntaxKind.EqualsToken)
                {
                    return ParseVariableDefinition();
                }
                else if (IsFunctionDefinition())
                {
                    return ParseFuncDef();
                }
                else
                {
                    return ParseExpression();
                }
            }
            else
            {
                return ParseExpression();
            }
        }

        private SyntaxNode ParseNumber()
        {
            var numberToken = MatchToken(SyntaxKind.Number);

            return new Number((double)numberToken.Value);
        }

        private SyntaxNode ParsePower()
        {
            var left = ParseFactor();
            if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Cirkum)
            {
                MatchToken(SyntaxKind.Cirkum);
                var right = ParseTerm();

                return new BinaryExpression { Left = left, Op = Operator.Power, Right = right };
            }

            return left;
        }

        private SyntaxNode ParseShortPower()
        {
            var atom = ParseAtom();
            if (Current.TokenKind<SyntaxKind>() == SyntaxKind.ShortPowerMinus)
            {
                MatchToken(SyntaxKind.ShortPowerMinus);
                var power = ParseShortPowerLiterals();

                return new ShortPowerNode(atom, power, true);
            }
            else if (Current.TokenKind<SyntaxKind>() == SyntaxKind.ShortPowerLiterals)
            {
                var power = ParseShortPowerLiterals();

                return new ShortPowerNode(atom, power, false);
            }

            return atom;
        }

        private SyntaxNode ParseShortPowerLiterals()
        {
            var token = MatchToken(SyntaxKind.ShortPowerLiterals);

            var charTable = new Dictionary<char, char> {
                { '⁰', '0' },
                { '¹', '1' },
                { '²', '2' },
                { '³', '3' },
                { '⁴', '4' },
                { '⁵', '5' },
                { '⁶', '6' },
                { '⁷', '7' },
                { '⁸', '8' },
                { '⁹', '9' }
            };

            var sb = new StringBuilder();

            foreach (var c in token.Text)
            {
                sb.Append(charTable[c]);
            }

            return new Number(double.Parse(sb.ToString()));
        }

        private SyntaxNode ParseTerm()
        {
            var left = ParsePower();
            if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Star)
            {
                MatchToken(SyntaxKind.Star);
                var right = ParseTerm();

                return new BinaryExpression { Left = left, Op = Operator.Multiplication, Right = right };
            }
            else if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Slash)
            {
                MatchToken(SyntaxKind.Slash);
                var right = ParseTerm();

                return new BinaryExpression { Left = left, Op = Operator.Division, Right = right };
            }
            else if (Current.TokenKind<SyntaxKind>() == SyntaxKind.Percentage)
            {
                MatchToken(SyntaxKind.Percentage);
                var right = ParseTerm();

                return new BinaryExpression { Left = left, Op = Operator.Modulo, Right = right };
            }

            return left;
        }

        private SyntaxNode ParseVariableDefinition()
        {
            var name = MatchToken(SyntaxKind.Identifier);

            MatchToken(SyntaxKind.EqualsToken);
            var expr = ParseExpression();

            return new VariableDefinitionNode(name.Text, expr);
        }
    }
}