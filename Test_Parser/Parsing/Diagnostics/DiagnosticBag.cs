using GrandTextAdventure.Core.Parsing.Text;
using GrandTextAdventure.Core.Parsing.Tokenizer;
using System.Collections;
using System.Collections.Generic;

namespace GrandTextAdventure.Core.Parsing.Diagnostics
{
    public sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new();

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _diagnostics.GetEnumerator();
        }

        public void ReportBadCharacter(TextSpan location, char character)
        {
            var message = $"Bad character input: '{character}'.";
            Report(location, message);
        }

        public void ReportUnexpectedDeclaration(TextSpan span, Token current)
        {
            var message = $"Unexpected Declaration: '{current.Kind}'";
            Report(span, message);
        }

        public void ReportUnexpectedToken(TextSpan span, object actualKind, object expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            Report(span, message);
        }

        private void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }
    }
}