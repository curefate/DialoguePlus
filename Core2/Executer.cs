namespace Narratoria.Core
{
    public class Executer
    {
        public void Execute(Runtime runtime, SIR sIR)
        {
            switch (sIR)
            {
                case SIR_Dialogue dialogue:
                    OnDialogue?.Invoke(runtime, dialogue);
                    break;
                case SIR_Menu menu:
                    OnMenu?.Invoke(runtime, menu);
                    break;
                case SIR_Jump jump:
                    OnJump?.Invoke(runtime, jump);
                    break;
                case SIR_Tour tour:
                    OnTour?.Invoke(runtime, tour);
                    break;
                case SIR_Call call:
                    OnCall?.Invoke(runtime, call);
                    break;
                case SIR_Assign assign:
                    OnAssign?.Invoke(runtime, assign);
                    break;
                case SIR_If ifStmt:
                    OnIf?.Invoke(runtime, ifStmt);
                    break;
                default:
                    throw new NotSupportedException($"(Runtime Error) Unsupported instruction type");
            }
        }

        public Action<Runtime, SIR_Dialogue> OnDialogue = (runtime, statement) =>
        {
            Console.WriteLine($"(Dialogue) {statement.Speaker}: {statement.Text.Evaluate(runtime)}");
        };

        public Action<Runtime, SIR_Menu> OnMenu = (runtime, statement) =>
        {
            Console.WriteLine("(Menu) Options:");
            for (int i = 0; i < statement.Options.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {statement.Options[i].Evaluate(runtime)}");
            }
            var input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= statement.Blocks.Count)
            {
                var selectedBlock = statement.Blocks[choice - 1];
                runtime.Enqueue(selectedBlock, true);
            }
            else
            {
                Console.WriteLine("(Menu) Invalid choice. No action taken.");
            }
        };

        private readonly Action<Runtime, SIR_Jump> OnJump = (runtime, statement) =>
        {
            try
            {
                var block = runtime.GetLabelBlock(statement.TargetLabel);
                runtime.ClearQueue();
                runtime.Enqueue(block.Statements);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"(Runtime Error) Label '{statement.TargetLabel}' not found.[Ln {statement.Line}]");
            }
        };

        private readonly Action<Runtime, SIR_Tour> OnTour = (runtime, statement) =>
        {
            try
            {
                var block = runtime.GetLabelBlock(statement.TargetLabel);
                runtime.Enqueue(block.Statements, true);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"(Runtime Error) Label '{statement.TargetLabel}' not found.[Ln {statement.Line}]");
            }
        };

        private readonly Action<Runtime, SIR_Call> OnCall = (runtime, statement) =>
        {
            try
            {
                var args = statement.Arguments.Select(arg => arg.Evaluate(runtime)).ToArray();
                runtime.Functions.Invoke(statement.FunctionName, args);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"(Runtime Error) Function '{statement.FunctionName}' not found.[Ln {statement.Line}]");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"(Runtime Error) Failed to call function '{statement.FunctionName}'. {ex.Message} [Ln {statement.Line}]", ex);
            }
        };

        private readonly Action<Runtime, SIR_Assign> OnAssign = (runtime, statement) =>
        {
            try
            {
                statement.Expression.Evaluate(runtime);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"(Runtime Error) Failed to evaluate assignment. {ex.Message} [Ln {statement.Line}]", ex);
            }
        };

        private readonly Action<Runtime, SIR_If> OnIf = (runtime, statement) =>
        {
            try
            {
                var conditionResult = statement.Condition.Evaluate(runtime);
                if (conditionResult == null || conditionResult is not bool)
                {
                    throw new InvalidOperationException($"(Runtime Error) Condition must evaluate to a boolean value.");
                }
                if ((bool)conditionResult)
                {
                    runtime.Enqueue(statement.ThenBlock, true);
                }
                else
                {
                    runtime.Enqueue(statement.ElseBlock, true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"(Runtime Error) Failed to evaluate if condition. {ex.Message} [Ln {statement.Line}]", ex);
            }
        };
    }
}