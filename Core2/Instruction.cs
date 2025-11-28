namespace Narratoria.Core
{
    public class SIRSet
    {
        public Dictionary<string, SIR_Label> Labels { get; } = [];
        public long Timestamp { get; init; }
        public string SourceFile { get; init; } = string.Empty;
        public string EntranceLabel { get; init; } = "__main__";
    }

    public abstract class SIR
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class SIR_Label : SIR
    {
        public required string LabelName { get; init; }
        public required string Source { get; init; }
        public List<SIR> Statements { get; } = [];
    }

    public class SIR_Dialogue : SIR
    {
        public string Speaker { get; init; } = string.Empty;
        public required FStringNode Text { get; init; }
        // public List<string> Tags { get; } = [];

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Speaker) ? "" : Speaker + ": ")}{Text}";
        }
    }

    public class SIR_Menu : SIR
    {
        public List<FStringNode> Options { get; } = [];
        public List<List<SIR>> Blocks { get; } = [];

        public override string ToString()
        {
            System.Text.StringBuilder sb = new();
            for (int i = 0; i < Options.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {Options[i]}:");
                foreach (var instr in Blocks[i])
                {
                    sb.AppendLine($"    {instr}");
                }
            }
            return sb.ToString();
        }
    }

    public class SIR_Jump : SIR
    {
        public required string TargetLabel { get; init; }

        public override string ToString()
        {
            return $"Jump to {TargetLabel}";
        }
    }

    public class SIR_Tour : SIR
    {
        public required string TargetLabel { get; init; }

        public override string ToString()
        {
            return $"Tour to {TargetLabel}";
        }
    }

    public class SIR_Call : SIR
    {
        public required string FunctionName { get; init; }
        public List<Expression> Arguments { get; } = [];

        public override string ToString()
        {
            return $"Call {FunctionName}({string.Join(", ", Arguments)})";
        }
    }

    public class SIR_Assign : SIR
    {
        public required Expression Expression { get; init; }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }

    public class SIR_If : SIR
    {
        public required Expression Condition { get; init; }
        public List<SIR> ThenBlock { get; } = [];
        public List<SIR> ElseBlock { get; } = [];

        public override string ToString()
        {
            System.Text.StringBuilder sb = new();
            sb.AppendLine($"If {Condition}:");
            foreach (var instr in ThenBlock)
            {
                sb.AppendLine($"    {instr}");
            }
            if (ElseBlock.Count > 0)
            {
                sb.AppendLine("Else:");
                foreach (var instr in ElseBlock)
                {
                    sb.AppendLine($"    {instr}");
                }
            }
            return sb.ToString();
        }
    }
}