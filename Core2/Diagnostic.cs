using System.Collections.Concurrent;

namespace Narratoria.Core
{
    // Diagnostic
    public abstract class BaseDiagnosticReporter : IDiagnosticReporter
    {
        protected readonly List<DiagnosticListener> _listeners = [];

        public virtual void Report(Diagnostic diagnostic)
        {
            foreach (var listener in _listeners)
            {
                listener.AddDiagnostic(diagnostic);
            }
        }

        public virtual void AttachDiagnosticListener(DiagnosticListener listener)
        {
            _listeners.Add(listener);
        }

        public virtual void DetachDiagnosticListener(DiagnosticListener listener)
        {
            _listeners.Remove(listener);
        }
    }

    public interface IDiagnosticReporter
    {
        void Report(Diagnostic diagnostic);
        void AttachDiagnosticListener(DiagnosticListener listener);
    }

    public class DiagnosticListener
    {
        private readonly ConcurrentQueue<Diagnostic> _bag = [];
        public readonly ConcurrentDictionary<Diagnostic.SeverityLevel, int> Counts = [];

        public virtual void AddDiagnostic(Diagnostic diagnostic)
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

    // Symbol Table
    public class SymbolPosition
    {
        public required string Uri { get; init; }
        public required int Line { get; init; }
        public required int Column { get; init; }
    }


    public class FileSymbolTable
    {
        public required string Uri { get; init; }
        public Dictionary<string, List<SymbolPosition>> LabelDefs { get; } = [];
        public Dictionary<string, List<SymbolPosition>> VariableDefs { get; } = [];
        public Dictionary<string, List<SymbolPosition>> LabelUsages { get; } = [];
        public Dictionary<string, List<SymbolPosition>> VariableUsages { get; } = [];
        public Dictionary<string, List<SymbolPosition>> References { get; } = [];

        public void AddLabelDef(string labelName, SymbolPosition position)
        {
            if (!LabelDefs.TryGetValue(labelName, out List<SymbolPosition>? value))
            {
                value = [];
                LabelDefs[labelName] = value;
            }
            value.Add(position);
        }

        public void AddVariableDef(string variableName, SymbolPosition position)
        {
            if (!VariableDefs.TryGetValue(variableName, out List<SymbolPosition>? value))
            {
                value = [];
                VariableDefs[variableName] = value;
            }
            value.Add(position);
        }

        public void AddLabelUsage(string labelName, SymbolPosition position)
        {
            if (!LabelUsages.TryGetValue(labelName, out List<SymbolPosition>? value))
            {
                value = [];
                LabelUsages[labelName] = value;
            }
            value.Add(position);
        }

        public void AddVariableUsage(string variableName, SymbolPosition position)
        {
            if (!VariableUsages.TryGetValue(variableName, out List<SymbolPosition>? value))
            {
                value = [];
                VariableUsages[variableName] = value;
            }
            value.Add(position);
        }

        public void AddReference(string referenceName, SymbolPosition position)
        {
            if (!References.TryGetValue(referenceName, out List<SymbolPosition>? value))
            {
                value = [];
                References[referenceName] = value;
            }
            value.Add(position);
        }
    }


    public class SymbolTableManager
    {
        private readonly Dictionary<string, FileSymbolTable> _fileTables = [];

        public void UpdateFileSymbols(FileSymbolTable table)
        {
            _fileTables[table.Uri] = table;
        }

        public void RemoveFileSymbols(string uri)
        {
            _fileTables.Remove(uri);
        }

        public bool ContainsFile(string uri)
        {
            return _fileTables.ContainsKey(uri);
        }

        public FileSymbolTable GetFileSymbolTable(string uri)
        {
            _fileTables.TryGetValue(uri, out var table);
            return table!;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new();
            foreach (var table in _fileTables.Values)
            {
                sb.AppendLine($"File: {table.Uri}");
                sb.AppendLine("  Label Defs:");
                foreach (var label in table.LabelDefs)
                {
                    sb.AppendLine($"    {label.Key} at Lines: {string.Join(", ", label.Value.Select(v => v.Line.ToString()))}");
                }
                sb.AppendLine("  Label Usages:");
                foreach (var label in table.LabelUsages)
                {
                    sb.AppendLine($"    {label.Key} at Lines: {string.Join(", ", label.Value.Select(v => v.Line.ToString()))}");
                }
                sb.AppendLine("  Variable Defs:");
                foreach (var variable in table.VariableDefs)
                {
                    sb.AppendLine($"    {variable.Key} at Lines: {string.Join(", ", variable.Value.Select(v => v.Line.ToString()))}");
                }
                sb.AppendLine("  Variable Usages:");
                foreach (var variable in table.VariableUsages)
                {
                    sb.AppendLine($"    {variable.Key} at Lines: {string.Join(", ", variable.Value.Select(v => v.Line.ToString()))}");
                }
            }
            return sb.ToString();
        }
    }
}