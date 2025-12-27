namespace Narratoria.Core
{
    public class Executer
    {
        private readonly LinkedList<SIR> _execQueue = new();
        private LabelSet? _currentSet = null;

        private readonly Runtime _runtime = new();
        public Runtime Runtime => _runtime;

        public virtual void Execute(LabelSet set, string? entranceLabel = null)
        {
            _currentSet = set;
            // Clear previous variables, but keep functions
            _runtime.Variables.Clear();
            _execQueue.Clear();

            if (entranceLabel != null)
            {
                if (!_currentSet.Labels.TryGetValue(entranceLabel, out SIR_Label? entrance))
                {
                    throw new KeyNotFoundException($"(Runtime Error) Entrance label '{entranceLabel}' not found in SIR set.");
                }
                Enqueue(entrance.Statements);
            }
            else
            {
                Enqueue(_currentSet.Labels[_currentSet.EntranceLabel].Statements);
            }

            while (_execQueue.Count > 0)
            {
                var instruction = Dequeue();
                Execute(instruction);
            }
        }

        private void Execute(SIR instruction)
        {
            switch (instruction)
            {
                case SIR_Dialogue dialogue:
                    OnDialogue?.Invoke(_runtime, dialogue);
                    break;
                case SIR_Menu menu:
                    int choice = OnMenu?.Invoke(_runtime, menu) ?? -1;
                    PostOnMenu(menu, choice);
                    break;
                case SIR_Jump jump:
                    ExecuteJump(jump);
                    break;
                case SIR_Tour tour:
                    ExecuteTour(tour);
                    break;
                case SIR_Call call:
                    ExecuteCall(call);
                    break;
                case SIR_Assign assign:
                    ExecuteAssign(assign);
                    break;
                case SIR_If ifStmt:
                    ExecuteIf(ifStmt);
                    break;
                default:
                    throw new NotSupportedException($"(Runtime Error) Unsupported instruction type");
            }
        }

        public Action<Runtime, SIR_Dialogue> OnDialogue = (runtime, statement) =>
        {
            Console.WriteLine($"(Dialogue) {statement.Speaker}: {statement.Text.Evaluate(runtime)}");
        };

        public Func<Runtime, SIR_Menu, int> OnMenu = (runtime, statement) =>
        {
            Console.WriteLine("(Menu) Options:");
            for (int i = 0; i < statement.Options.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {statement.Options[i].Evaluate(runtime)}");
            }
            var input = Console.ReadLine();
            int choice;
            while (!int.TryParse(input, out choice) || choice < 1 || choice > statement.Options.Count)
            {
                Console.WriteLine("Invalid choice. Please enter a valid option number.");
                input = Console.ReadLine();
            }
            return choice - 1;
        };

        private void PostOnMenu(SIR_Menu statement, int choice)
        {
            if (choice < 0 || choice >= statement.Blocks.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(choice), "Choice index is out of range.");
            }
            var selectedBlock = statement.Blocks[choice];
            Enqueue(selectedBlock, true);
        }

        private void ExecuteJump(SIR_Jump statement)
        {

            var target = _currentSet?.Labels[statement.TargetLabel] ?? throw new KeyNotFoundException($"(Runtime Error) Label '{statement.TargetLabel}' not found.[Ln {statement.Line}]");
            _execQueue.Clear();
            Enqueue(target.Statements);
        }

        private void ExecuteTour(SIR_Tour statement)
        {

            var target = _currentSet?.Labels[statement.TargetLabel] ?? throw new KeyNotFoundException($"(Runtime Error) Label '{statement.TargetLabel}' not found.[Ln {statement.Line}]");
            Enqueue(target.Statements, true);

        }

        private void ExecuteCall(SIR_Call statement)
        {
            try
            {
                var args = statement.Arguments.Select(arg => arg.Evaluate(_runtime)).ToArray();
                _runtime.Functions.Invoke(statement.FunctionName, args);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"(Runtime Error) Function '{statement.FunctionName}' not found.[Ln {statement.Line}]");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"(Runtime Error) Failed to call function '{statement.FunctionName}'. {ex.Message} [Ln {statement.Line}]", ex);
            }
        }

        private void ExecuteAssign(SIR_Assign statement)
        {
            try
            {
                statement.Expression.Evaluate(_runtime);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"(Runtime Error) Failed to evaluate assignment. {ex.Message} [Ln {statement.Line}]", ex);
            }
        }

        private void ExecuteIf(SIR_If statement)
        {
            try
            {
                var conditionResult = statement.Condition.Evaluate(_runtime);
                if (conditionResult == null || conditionResult is not bool)
                {
                    throw new InvalidOperationException($"(Runtime Error) Condition must evaluate to a boolean value.");
                }
                if ((bool)conditionResult)
                {
                    Enqueue(statement.ThenBlock, true);
                }
                else
                {
                    Enqueue(statement.ElseBlock, true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"(Runtime Error) Failed to evaluate if condition. {ex.Message} [Ln {statement.Line}]", ex);
            }
        }

        private void Enqueue(List<SIR> instructions, bool toFront = false)
        {
            if (instructions == null || instructions.Count == 0)
            {
                return;
            }
            if (toFront)
            {
                for (int i = instructions.Count - 1; i >= 0; i--)
                {
                    _execQueue.AddFirst(instructions[i]);
                }
            }
            else
            {
                foreach (var instruction in instructions)
                {
                    _execQueue.AddLast(instruction);
                }
            }
        }

        private SIR Dequeue()
        {
            if (_execQueue.Count == 0)
            {
                throw new InvalidOperationException("Execution queue is empty.");
            }
            var instruction = _execQueue.First?.Value ?? throw new InvalidOperationException("Execution queue contains a null instruction.");
            _execQueue.RemoveFirst();
            return instruction;
        }
    }
}