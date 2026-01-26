namespace DialoguePlus.Core
{
    /// <summary>
    /// A collection of compiled labels that can be executed by the <see cref="Executer"/>.
    /// </summary>
    public class LabelSet
    {
        /// <summary>
        /// The default entrance label name for top-level statements.
        /// </summary>
        public static readonly string DefaultEntranceLabel = "@system/__main__";
        /// <summary>
        /// Gets the dictionary of all labels in this set, keyed by label name.
        /// </summary>
        public Dictionary<string, SIR_Label> Labels { get; } = [];
        /// <summary>
        /// Gets or initializes the entrance label name for execution. Defaults to <see cref="DefaultEntranceLabel"/>.
        /// </summary>
        public string EntranceLabel { get; init; } = DefaultEntranceLabel;
    }

    /// <summary>
    /// Base class for all Structured Intermediate Representation (SIR) instructions.
    /// </summary>
    public abstract class SIR
    {
        /// <summary>
        /// Gets or sets the source line number where this instruction was defined.
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// Gets or sets the source column number where this instruction was defined.
        /// </summary>
        public int Column { get; set; }
    }

    /// <summary>
    /// Represents a label definition containing a block of statements.
    /// </summary>
    public class SIR_Label : SIR
    {
        /// <summary>
        /// Gets the name of this label.
        /// </summary>
        public required string LabelName { get; init; }
        /// <summary>
        /// Gets the source ID (URI) where this label was defined.
        /// </summary>
        public required string SourceID { get; init; }
        /// <summary>
        /// Gets the list of statements in this label block.
        /// </summary>
        public List<SIR> Statements { get; init; } = [];
    }

    /// <summary>
    /// Represents a dialogue statement with optional speaker and text content.
    /// </summary>
    public class SIR_Dialogue : SIR
    {
        /// <summary>
        /// Gets the speaker name for this dialogue (empty if no speaker).
        /// </summary>
        public string Speaker { get; init; } = string.Empty;
        /// <summary>
        /// Gets the formatted text content of the dialogue.
        /// </summary>
        public required FStringNode Text { get; init; }
        // public List<string> Tags { get; } = [];

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Speaker) ? "" : Speaker + ": ")}{Text}";
        }
    }

    /// <summary>
    /// Represents a menu (choice) statement with multiple options and corresponding code blocks.
    /// </summary>
    public class SIR_Menu : SIR
    {
        /// <summary>
        /// Gets the list of option texts displayed to the user.
        /// </summary>
        public List<FStringNode> Options { get; } = [];
        /// <summary>
        /// Gets the list of code blocks corresponding to each option.
        /// </summary>
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

    /// <summary>
    /// Represents a jump instruction that transfers control to another label.
    /// Clears the execution queue and resets the variable scope.
    /// </summary>
    public class SIR_Jump : SIR
    {
        /// <summary>
        /// Gets the target label name to jump to.
        /// </summary>
        public required string TargetLabel { get; init; }

        public override string ToString()
        {
            return $"Jump to {TargetLabel}";
        }
    }

    /// <summary>
    /// Represents a tour instruction (like a subroutine call) that transfers control to another label.
    /// Creates a new variable scope and returns after the label completes.
    /// </summary>
    public class SIR_Tour : SIR
    {
        /// <summary>
        /// Gets the target label name to tour to.
        /// </summary>
        public required string TargetLabel { get; init; }

        public override string ToString()
        {
            return $"Tour to {TargetLabel}";
        }
    }

    /// <summary>
    /// Represents a function call instruction.
    /// </summary>
    public class SIR_Call : SIR
    {
        /// <summary>
        /// Gets the name of the function to call.
        /// </summary>
        public required string FunctionName { get; init; }
        /// <summary>
        /// Gets the list of argument expressions to pass to the function.
        /// </summary>
        public List<DPExpression> Arguments { get; } = [];

        public override string ToString()
        {
            return $"Call {FunctionName}({string.Join(", ", Arguments)})";
        }
    }

    /// <summary>
    /// Represents an assignment or expression evaluation instruction.
    /// </summary>
    public class SIR_Assign : SIR
    {
        /// <summary>
        /// Gets the expression to evaluate (may include variable assignments).
        /// </summary>
        public required DPExpression Expression { get; init; }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }

    /// <summary>
    /// Represents a conditional (if-else) instruction.
    /// </summary>
    public class SIR_If : SIR
    {
        /// <summary>
        /// Gets the condition expression that must evaluate to a boolean.
        /// </summary>
        public required DPExpression Condition { get; init; }
        /// <summary>
        /// Gets the list of statements to execute if the condition is true.
        /// </summary>
        public List<SIR> ThenBlock { get; } = [];
        /// <summary>
        /// Gets the list of statements to execute if the condition is false.
        /// </summary>
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

    internal class Internal_SIR_Pop : SIR
    {
        static public readonly Internal_SIR_Pop Instance = new();
    }
}