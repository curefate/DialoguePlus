namespace Narratoria.Core
{
    public class AST
    {
        public required string Source { get; init; }
        public List<LabelNode> Nodes { get; } = [];
    }

    public interface IASTVisitor<T>
    {
        T Visit(ASTNode node);
    }

    public abstract class ASTNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public virtual T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class LabelNode : ASTNode
    {
        public required string Label { get; init; }
        public required string Source { get; init; }
        public List<ASTNode> Nodes { get; } = [];
    }

    public class DialogueNode : ASTNode
    {
        public string Speaker { get; init; } = string.Empty;
        public required FStringNode Text { get; init; }
        public List<string> Tags { get; } = [];

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Speaker) ? "" : Speaker + ": ")}{Text}";
        }
    }

    public class MenuNode : ASTNode
    {
        public List<FStringNode> Options { get; } = [];
        public List<List<ASTNode>> Blocks { get; } = [];

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

    public class JumpNode : ASTNode
    {
        public required string Target { get; init; }

        public override string ToString()
        {
            return $"Jump to {Target}";
        }
    }

    public class TourNode : ASTNode
    {
        public required string Target { get; init; }

        public override string ToString()
        {
            return $"Tour to {Target}";
        }
    }

    public class CallNode : ASTNode
    {
        public required string FunctionName { get; init; }
        public List<Expression> Arguments { get; } = [];

        public override string ToString()
        {
            return $"Call {FunctionName}({string.Join(", ", Arguments)})";
        }
    }

    public class AssignNode : ASTNode
    {
        public required Expression Expression { get; init; }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }

    public class IfNode : ASTNode
    {
        public required Expression Condition { get; init; }
        public List<ASTNode> ThenBlock { get; } = [];
        public List<ASTNode> ElseBlock { get; } = [];

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