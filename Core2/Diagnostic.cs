using System.Collections.Concurrent;

namespace Narratoria.Core
{
    // Diagnostic
    public interface IDiagnosticReporter
    {
        void Report(Diagnostic diagnostic);
        void AddCollector(DiagnosticCollector collector);
    }

    public class DiagnosticCollector
    {
        private readonly ConcurrentBag<Diagnostic> _bag = [];

        public void Add(Diagnostic diagnostic)
        {
            _bag.Add(diagnostic);
        }
        
        public List<Diagnostic> GetAll()
        {
            return [.. _bag];
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
            Info = 3
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
        public required string FilePath { get; init; }
        public required int Line { get; init; }
        public required int Column { get; init; }
    }


    public class FileSymbolTable
    {
        public required string FilePath { get; init; }

        public Dictionary<string, SymbolPosition> LabelDefs { get; } = [];

        public Dictionary<string, SymbolPosition> VariableDefs { get; } = [];

        public Dictionary<string, List<SymbolPosition>> LabelUsages { get; } = [];

        public Dictionary<string, List<SymbolPosition>> VariableUsages { get; } = [];

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
    }


    public class SymbolTableManager
    {
        private readonly Dictionary<string, FileSymbolTable> _fileTables = [];

        public void UpdateFileSymbols(FileSymbolTable table)
        {
            _fileTables[table.FilePath] = table;
        }

        public void RemoveFileSymbols(string filePath)
        {
            _fileTables.Remove(filePath);
        }

        public bool ContainsFile(string filePath)
        {
            return _fileTables.ContainsKey(filePath);
        }

        public SymbolPosition? FindDefinition(string symbolName)
        {
            foreach (var table in _fileTables.Values)
            {
                if (table.LabelDefs.TryGetValue(symbolName, out var labelDef)) return labelDef;
                if (table.VariableDefs.TryGetValue(symbolName, out var varDef)) return varDef;
            }
            return null;
        }

        public List<(string FilePath, int Line, int Column)> FindReferences(string symbolName)
        {
            var results = new List<(string FilePath, int Line, int Column)>();
            foreach (var table in _fileTables.Values)
            {
                if (table.LabelUsages.TryGetValue(symbolName, out var labelRefs))
                {
                    results.AddRange(labelRefs.Select(pos => (table.FilePath, pos.Line, pos.Column)));
                }
                if (table.VariableUsages.TryGetValue(symbolName, out var varRefs))
                {
                    results.AddRange(varRefs.Select(pos => (table.FilePath, pos.Line, pos.Column)));
                }
            }
            return results;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new();
            foreach (var table in _fileTables.Values)
            {
                sb.AppendLine($"File: {table.FilePath}");
                sb.AppendLine("  Label Defs:");
                foreach (var label in table.LabelDefs)
                {
                    sb.AppendLine($"    {label.Key} at Line {label.Value.Line}, Column {label.Value.Column}");
                }
                sb.AppendLine("  Label Usages:");
                foreach (var label in table.LabelUsages)
                {
                    sb.AppendLine($"    {label.Key} at Lines: {string.Join(", ", label.Value.Select(v => v.Line.ToString()))}");
                }
                sb.AppendLine("  Variable Defs:");
                foreach (var variable in table.VariableDefs)
                {
                    sb.AppendLine($"    {variable.Key} at Line {variable.Value.Line}, Column {variable.Value.Column}");
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