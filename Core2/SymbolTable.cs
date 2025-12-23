namespace Narratoria.Core
{
    public class SymbolPosition
    {
        public required string SourceID { get; init; }
        public required string Label { get; init; }
        public required int Line { get; init; }
        public required int Column { get; init; }
    }

    public class FileSymbolTable
    {
        public required string SourceID { get; init; }
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
            _fileTables[table.SourceID] = table;
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
                sb.AppendLine($"File: {table.SourceID}");
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