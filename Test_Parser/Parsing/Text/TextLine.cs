namespace GrandTextAdventure.Core.Parsing.Text
{
    public sealed class TextLine
    {
        public TextLine(SourceText text, int start, int length, int lengthIncludingLineBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthIncludingLineBreak = lengthIncludingLineBreak;
        }

        public int End => Start + Length;
        public int Length { get; }
        public int LengthIncludingLineBreak { get; }
        public TextSpan Span => new(Start, Length);
        public TextSpan SpanIncludingLineBreak => new(Start, LengthIncludingLineBreak);
        public int Start { get; }
        public SourceText Text { get; }

        public override string ToString() => Text.ToString(Span);
    }
}