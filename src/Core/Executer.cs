using System.Threading;
using System.Threading.Tasks;

namespace DialoguePlus.Core
{
    /// <summary>
    /// Executes compiled DialoguePlus scripts by processing SIR (Structured Intermediate Representation) instructions.
    /// </summary>
    public class Executer
    {
        private readonly LinkedList<SIR> _execQueue = new();
        private LabelSet? _currentSet = null;
        private readonly Runtime _runtime;
        /// <summary>
        /// Gets the runtime instance managing variables and functions for this executer.
        /// </summary>
        public Runtime Runtime => _runtime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Executer"/> class.
        /// </summary>
        /// <param name="runtime">The runtime instance to use. If null, a new runtime will be created.</param>
        public Executer(Runtime? runtime = null)
        {
            _runtime = runtime ?? new Runtime();
        }

        /// <summary>
        /// Prepares a label set for execution by loading it and initializing the execution queue.
        /// </summary>
        /// <param name="set">The label set to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when set is null.</exception>
        public void Prepare(LabelSet set)
        {
            _currentSet = set ?? throw new ArgumentNullException(nameof(set));
            _execQueue.Clear();

            Enqueue(_currentSet.Labels[_currentSet.EntranceLabel].Statements);
        }

        /// <summary>
        /// Automatically steps through execution based on the specified mode.
        /// Mode 0: Executes all instructions until completion.
        /// Mode 1: Steps until reaching a dialogue, menu, or call instruction.
        /// </summary>
        /// <param name="mode">The execution mode (0 or 1).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when mode is not 0 or 1.</exception>
        public async Task AutoStepAsync(int mode = 0, CancellationToken ct = default)
        {
            switch (mode)
            {
                case 0:
                    while (await StepAsync(ct)) ;
                    break;
                case 1:
                    while (HasNext)
                    {
                        if (Peek() is SIR_Dialogue || Peek() is SIR_Menu || Peek() is SIR_Call)
                        {
                            break;
                        }
                        await StepAsync(ct);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), "Invalid auto step mode. mode 0 = step to end, mode 1 = step to next dialogue/menu");
            }
        }

        /// <summary>
        /// Executes the next instruction in the queue.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if there are more instructions in the queue; false if the queue is empty.</returns>
        public async Task<bool> StepAsync(CancellationToken ct = default)
        {
            if (!HasNext) return false;

            var instruction = Dequeue();
            ct.ThrowIfCancellationRequested();

            switch (instruction)
            {
                case SIR_Dialogue dialogue:
                    var dlgTask = OnDialogueAsync?.Invoke(_runtime, dialogue) ?? Task.CompletedTask;
                    await dlgTask;
                    break;
                case SIR_Menu menu:
                    var menuTask = OnMenuAsync?.Invoke(_runtime, menu);
                    int choice = menuTask != null ? await menuTask : -1;
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
        /// Gets or sets the asynchronous callback invoked when a dialogue statement is executed.
        /// The callback receives the runtime and dialogue statement, and should return a completed task when done.
        /// </summary>
        public Func<Runtime, SIR_Dialogue, Task> OnDialogueAsync { get; set; } =
            (runtime, statement) =>
            {
                // Default implementation
                Console.WriteLine($"(Dialogue) {statement.Speaker}: {statement.Text.Evaluate(runtime)}");
                return Task.CompletedTask;
            };

        /// <summary>
        /// Gets or sets the asynchronous callback invoked when a menu statement is executed.
        /// The callback receives the runtime and menu statement, and should return the zero-based index of the selected option.
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

        /// <summary>
        /// Gets whether there are more instructions to execute in the queue.
        /// </summary>
        public bool HasNext => _execQueue.Count > 0;
        /// <summary>
        /// Peeks at the next instruction in the queue without removing it.
        /// </summary>
        /// <returns>The next instruction, or null if the queue is empty.</returns>
        public SIR? Peek() => _execQueue.First?.Value;
    }
}
