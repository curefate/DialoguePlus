namespace Narratoria.Core
{
    public class SemanticTree
    {
        public required string Source { get; init; }
        public List<SemanticLabel> Labels { get; } = [];
    }

    public abstract class SemanticNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class SemanticLabel : SemanticNode
    {
        public required string LabelName { get; init; }
        public required string Source { get; init; }
        public List<SemanticNode> Nodes { get; } = [];
    }

    public class SemanticDialogue : SemanticNode
    {
        public string Speaker { get; init; } = string.Empty;
        public required FStringNode Text { get; init; }
        public List<string> Tags { get; } = [];

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Speaker) ? "" : Speaker + ": ")}{Text}";
        }
    }

    public class SemanticMenu : SemanticNode
    {
        public List<FStringNode> Options { get; } = [];
        public List<List<SemanticNode>> Blocks { get; } = [];

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

    public class SemanticJump : SemanticNode
    {
        public required string Target { get; init; }

        public override string ToString()
        {
            return $"Jump to {Target}";
        }
    }

    public class SemanticTour : SemanticNode
    {
        public required string Target { get; init; }

        public override string ToString()
        {
            return $"Tour to {Target}";
        }
    }

    public class SemanticCall : SemanticNode
    {
        public required string FunctionName { get; init; }
        public List<Expression> Arguments { get; } = [];

        public override string ToString()
        {
            return $"Call {FunctionName}({string.Join(", ", Arguments)})";
        }
    }

    public class SemanticAssign : SemanticNode
    {
        public required Expression Expression { get; init; }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }

    public class SemanticIf : SemanticNode
    {
        public required Expression Condition { get; init; }
        public List<SemanticNode> ThenBlock { get; } = [];
        public List<SemanticNode> ElseBlock { get; } = [];

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