using System.Threading;
using System.Threading.Tasks;

namespace DialoguePlus.Core
{
    public class Executer
    {
        private readonly LinkedList<SIR> _execQueue = new();
        private LabelSet? _currentSet = null;
        private readonly Runtime _runtime;
        public Runtime Runtime => _runtime;

        public Executer(Runtime? runtime = null)
        {
            _runtime = runtime ?? new Runtime();
        }

        public void Prepare(LabelSet set)
        {
            _currentSet = set ?? throw new ArgumentNullException(nameof(set));
            _execQueue.Clear();

            Enqueue(_currentSet.Labels[_currentSet.EntranceLabel].Statements);
        }

        /// <summary>
        /// Automatically steps
        /// mode 0 = step to end, mode 1 = step to next dialogue/menu
        /// </summary>
        public async Task AutoStepAsync(int mode = 0, CancellationToken ct = default)
        {
            switch (mode)
            {
                case 0:
                    while (await StepAsync(ct).ConfigureAwait(false)) ;
                    break;
                case 1:
                    while (HasNext)
                    {
                        if (Peek() is SIR_Dialogue || Peek() is SIR_Menu)
                        {
                            break;
                        }
                        await StepAsync(ct).ConfigureAwait(false);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), "Invalid auto step mode. mode 0 = step to end, mode 1 = step to next dialogue/menu");
            }
        }

        /// <summary>
        /// Execute the next instruction in the queue.
        /// Returns true if there are more instructions in the queue; false if empty.
        /// </summary>
        public async Task<bool> StepAsync(CancellationToken ct = default)
        {
            if (!HasNext) return false;

            var instruction = Dequeue();
            ct.ThrowIfCancellationRequested();

            switch (instruction)
            {
                case SIR_Dialogue dialogue:
                    var dlgTask = OnDialogueAsync?.Invoke(_runtime, dialogue) ?? Task.CompletedTask;
                    await dlgTask.ConfigureAwait(false);
                    break;
                case SIR_Menu menu:
                    var menuTask = OnMenuAsync?.Invoke(_runtime, menu);
                    int choice = menuTask != null ? await menuTask.ConfigureAwait(false) : -1;
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
                case Internal_SIR_Pop:
                    _runtime.Variables.PopTempScope();
                    break;
                default:
                    throw new NotSupportedException("(Runtime Error) Unsupported instruction type");
            }

            return HasNext;
        }

        /// <summary>
        /// Asynchronous callback for dialogue statements.
        /// </summary>
        public Func<Runtime, SIR_Dialogue, Task> OnDialogueAsync { get; set; } =
            (runtime, statement) =>
            {
                // Default implementation
                Console.WriteLine($"(Dialogue) {statement.Speaker}: {statement.Text.Evaluate(runtime)}");
                return Task.CompletedTask;
            };

        /// <summary>
        /// Asynchronous callback for menu statements.
        /// </summary>
        public Func<Runtime, SIR_Menu, Task<int>> OnMenuAsync { get; set; } =
            async (runtime, statement) =>
            {
                // Default implementation
                Console.WriteLine("(Menu) Options:");
                for (int i = 0; i < statement.Options.Count; i++)
                {
                    Console.WriteLine($" {i + 1}. {statement.Options[i].Evaluate(runtime)}");
                }

                int choice = -1;
                while (true)
                {
                    Console.Write("Enter choice #: ");
                    var input = Console.ReadLine();
                    if (int.TryParse(input, out choice) &&
                        choice >= 1 && choice <= statement.Options.Count)
                    {
                        break;
                    }
                    Console.WriteLine("Invalid choice. Please enter a valid option number.");
                    await Task.Yield();
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
            Enqueue(selectedBlock, toFront: true);
        }

        private void ExecuteJump(SIR_Jump statement)
        {
            var target = _currentSet?.Labels[statement.TargetLabel]
                ?? throw new KeyNotFoundException($"(Runtime Error) Label '{statement.TargetLabel}' not found.[Ln {statement.Line}]");

            _execQueue.Clear();
            _runtime.Variables.PopTempScope();
            Enqueue(target.Statements);
        }

        private void ExecuteTour(SIR_Tour statement)
        {
            var target = _currentSet?.Labels[statement.TargetLabel]
                ?? throw new KeyNotFoundException($"(Runtime Error) Label '{statement.TargetLabel}' not found.[Ln {statement.Line}]");

            _runtime.Variables.NewTempScope();
            _execQueue.AddFirst(Internal_SIR_Pop.Instance);
            Enqueue(target.Statements, toFront: true);
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
                    throw new InvalidOperationException("(Runtime Error) Condition must evaluate to a boolean value.");
                }

                if ((bool)conditionResult)
                {
                    Enqueue(statement.ThenBlock, toFront: true);
                }
                else
                {
                    Enqueue(statement.ElseBlock, toFront: true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"(Runtime Error) Failed to evaluate if condition. {ex.Message} [Ln {statement.Line}]", ex);
            }
        }

        private void Enqueue(List<SIR> instructions, bool toFront = false)
        {
            if (instructions == null || instructions.Count == 0) return;

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
            var instruction = _execQueue.First?.Value
                ?? throw new InvalidOperationException("Execution queue contains a null instruction.");
            _execQueue.RemoveFirst();
            return instruction;
        }

        public bool HasNext => _execQueue.Count > 0;
        public SIR? Peek() => _execQueue.First?.Value;
    }
}
