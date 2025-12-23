using System.Collections.Concurrent;

namespace Narratoria.Core
{
    // Diagnostic
    public class DiagnosticEngine
    {
        private readonly ConcurrentQueue<Diagnostic> _bag = [];
        public readonly ConcurrentDictionary<Diagnostic.SeverityLevel, int> Counts = [];

        public virtual void Report(Diagnostic diagnostic)
        {
            _bag.Enqueue(diagnostic);
            if (Counts.TryGetValue(diagnostic.Severity, out int value))
            {
                Counts[diagnostic.Severity] = ++value;
            }
            else
            {
                Counts.AddOrUpdate(diagnostic.Severity, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public List<Diagnostic> GetAll()
        {
            var list = new List<Diagnostic>();
            while (!_bag.IsEmpty)
            {
                if (_bag.TryDequeue(out Diagnostic? diag))
                {
                    list.Add(diag);
                }
            }
            return list;
        }

        public void Clear()
        {
            while (!_bag.IsEmpty)
            {
                _bag.TryDequeue(out _);
            }
        }
    }

    public class Diagnostic
    {
        public required string Message { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }
        public TextSpan? Span { get; init; }
        public SeverityLevel Severity { get; init; }

        public enum SeverityLevel
        {
            Error = 1,
            Warning = 2,
            Info = 3,
            Log = 4
        }

        public override string ToString()
        {
            return $"[{Severity}]".PadRight(10) + $"{Message} [Ln {Line}, Col {Column}]";
        }
    }

    public readonly struct TextSpan
    {
        public required int StartLine { get; init; }
        public required int StartColumn { get; init; }
        public required int EndLine { get; init; }
        public required int EndColumn { get; init; }
    }
}