using GrandTextAdventure.Core.Parsing;
using System;
using System.Globalization;

namespace Test_Parser
{
    internal class Program
    {
        private static int indent = 0;

        private static void DisplayTree(SyntaxNode node)
        {
            if (node is Number numNode)
            {
                Console.WriteLine(new string('\t', indent) + "Number: " + numNode.Value.ToString(CultureInfo.InvariantCulture));
            }
            else if (node is BinaryExpression binNode)
            {
                Console.WriteLine(new string('\t', indent) + "BinaryExpression: ");
                indent++;
                DisplayTree(binNode.Left);
                Console.WriteLine(new string('\t', indent) + "Operator: " + binNode.Op);
                DisplayTree(binNode.Right);
                indent--;
            }
            else if (node is GroupExpression groupNode)
            {
                indent++;
                Console.WriteLine(new string('\t', indent) + "Group: ");
                DisplayTree(groupNode.Expression);
                indent--;
            }
        }

        private static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                Parser parser = new();

                var tree = parser.Parse(input);
                var value = Evaluator.Evaluate(tree);

                DisplayTree(tree);

                Console.WriteLine("Ergebnis: " + value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}