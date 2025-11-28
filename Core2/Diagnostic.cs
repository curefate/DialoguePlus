namespace Narratoria.Core2
{
    public class Diagnostic
    {
        public string Message { get; init; } = string.Empty;
        public int Line { get; init; }
        public int Column { get; init; }
        public int Severity { get; init; } = 1; // 1: Error, 2: Warning, 3: Info
    }
}