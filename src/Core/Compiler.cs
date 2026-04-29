// Licensed under the MIT License.
// See LICENSE file in the project root.

using System.Text;

namespace DialoguePlus.Core
{
    /// <summary>
    /// Represents a compilation request.
    /// </summary>
    public sealed record CompileRequest
    {
        /// <summary>
        /// The entry sourceId of the script to compile.
        /// </summary>
        public required string EntrySourceId { get; init; }

        /// <summary>
        /// Optional in-memory text for the entry document.
        /// When provided, this text is used for the entry sourceId instead of querying the content resolver.
        /// </summary>
        public string? EntryTextOverride { get; init; }

        /// <summary>
        /// Cancellation token for the compilation.
        /// </summary>
        public CancellationToken CancellationToken { get; init; } = default;
    }

    /// <summary>
    /// The main compiler for DialoguePlus scripts (.dp files).
    /// Compiles source files into executable label sets and manages symbol tables.
    /// </summary>
    public class Compiler
    {
        private readonly IScriptResolver _resolver;

        private readonly SymbolTableManager _symbolTableManager = new();
        /// <summary>
        /// Gets the symbol table manager containing symbol information for all compiled files.
        /// </summary>
        public SymbolTableManager SymbolTables => _symbolTableManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Compiler"/> class.
        /// </summary>
        /// <param name="resolver">The script resolver to use for loading sources and resolving imports.</param>
        public Compiler(IScriptResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// Compiles a DialoguePlus script from an entry sourceId.
        /// </summary>
        /// <param name="request">Compilation request.</param>
        /// <returns>A <see cref="CompileResult"/> containing the compilation status, diagnostics, and compiled label set.</returns>
        public CompileResult Compile(CompileRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return CompileAsync(request).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Compiles a DialoguePlus script asynchronously.
        /// </summary>
        public async Task<CompileResult> CompileAsync(CompileRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var session = new CompilationSession(request.EntrySourceId, _resolver, request.EntryTextOverride);
            var result = await session.CompileAsync(request.CancellationToken).ConfigureAwait(false);
            _symbolTableManager.Merge(session.SymbolTables);
            return result;
        }
    }

    /// <summary>
    /// Represents the result of a compilation operation.
    /// </summary>
    public sealed record CompileResult
    {
        /// <summary>
        /// Gets whether the compilation was successful (no errors).
        /// </summary>
        public required bool Success { get; init; }
        /// <summary>
        /// Gets the list of diagnostics (errors, warnings, info) generated during compilation.
        /// </summary>
        public required List<Diagnostic> Diagnostics { get; init; }
        /// <summary>
        /// Gets the compiled label set containing all executable labels and statements.
        /// </summary>
        public required LabelSet Labels { get; init; }
        /// <summary>
        /// Gets the source ID (URI) of the main compiled file.
        /// </summary>
        public required string SourceID { get; init; }
        /// <summary>
        /// Gets the Unix timestamp (milliseconds) when the compilation was performed.
        /// </summary>
        public long Timestamp { get; init; }
    }

    internal class CompilationSession
    {
        private readonly IScriptResolver _resolver;
        private readonly string? _entryTextOverride;

        public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public string SourceID { get; init; }

        public DiagnosticEngine Diagnostics { get; init; } = new DiagnosticEngine();
        public SymbolTableManager SymbolTables { get; init; } = new SymbolTableManager();
        public Dictionary<string, LabelSet> ImportedLabelSets { get; init; } = [];

        public CompilationSession(string sourceID, IScriptResolver resolver, string? entryTextOverride = null)
        {
            SourceID = sourceID;
            _resolver = resolver;
            _entryTextOverride = entryTextOverride;
        }

        private async Task<string> GetSourceTextAsync(string sourceId, CancellationToken cancellationToken = default)
        {
            // Use the entry override only for the main entry sourceId.
            if (_entryTextOverride != null && sourceId == SourceID)
            {
                return _entryTextOverride;
            }

            var context = await _resolver.Content.GetTextAsync(sourceId, cancellationToken).ConfigureAwait(false);
            return context.Text;
        }

        // 后续再根据入口标签优化，现在先简单合并全部
        private LabelSet CollectLabels()
        {
            LabelSet resultSet = new();
            foreach (var labelSet in ImportedLabelSets.Values)
            {
                foreach (var label in labelSet.Labels)
                {
                    if (!resultSet.Labels.ContainsKey(label.Key))
                    {
                        resultSet.Labels.Add(label.Key, label.Value);
                    }
                }
            }
            return resultSet;
        }

        private async Task CompileInternalAsync(string sourceId, string code, CancellationToken cancellationToken = default, int line = -1, int column = -1)
        {
            if (ImportedLabelSets.ContainsKey(sourceId))
            {
                return;
            }

            var diagnostics = sourceId == SourceID ? Diagnostics : new DiagnosticEngine();
            var lexer = new Lexer(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(code))), diagnostics);
            var parser = new Parser(lexer, diagnostics);
            var ast = parser.Parse();

            var table = new FileSymbolTable { SourceID = sourceId };
            var labelSet = new LabelSet();
            ImportedLabelSets.Add(sourceId, labelSet); // Prevent circular import
            var builder = new IRBuilder(table);

            foreach (var import in ast.Imports)
            {
                var importedSourceId = _resolver.Imports.Resolve(sourceId, import.Path.Lexeme);
                try
                {
                    var importCode = await GetSourceTextAsync(importedSourceId, cancellationToken).ConfigureAwait(false);
                    await CompileInternalAsync(importedSourceId, importCode, cancellationToken, import.Path.Line, import.Path.Column).ConfigureAwait(false);
                    table.AddReference(importedSourceId, new SymbolPosition
                    {
                        SourceID = sourceId,
                        Label = null,
                        Line = import.Path.Line,
                        Column = import.Path.Column,
                    });
                }
                catch (Exception ex)
                {
                    diagnostics?.Report(new Diagnostic
                    {
                        Message = $"[Compiler] Failed to import '{import.Path.Lexeme}': {ex.Message}",
                        Line = import.Path.Line,
                        Column = import.Path.Column,
                        Span = new TextSpan()
                        {
                            StartLine = import.Path.Line,
                            StartColumn = import.Path.Column,
                            EndLine = import.Path.Line,
                            EndColumn = import.Path.Column + import.Path.Lexeme.Length,
                        },
                        Severity = Diagnostic.SeverityLevel.Error
                    });
                    continue;
                }
            }

            if (ast.TopLevelStatements.Count > 0 && sourceId == SourceID)
            {
                SIR_Label topLabel = new()
                {
                    LabelName = LabelSet.DefaultEntranceLabel,
                    SourceID = sourceId,
                    Line = -1,
                    Column = -1,
                };
                foreach (var stmt in ast.TopLevelStatements)
                {
                    var sir = builder.Visit(stmt);
                    if (sir != null)
                    {
                        topLabel.Statements.Add(sir);
                    }
                }
                labelSet.Labels.Add(LabelSet.DefaultEntranceLabel, topLabel);
            }

            foreach (var label in ast.Labels)
            {
                var labelblock = (SIR_Label)builder.Visit(label);
                if (!labelSet.Labels.ContainsKey(labelblock.LabelName))
                {
                    labelSet.Labels.Add(labelblock.LabelName, labelblock);
                }
                else
                {
                    labelSet.Labels[labelblock.LabelName].Statements.AddRange(labelblock.Statements);
                }
                if (labelblock.Statements.Count == 0)
                {
                    diagnostics?.Report(new Diagnostic
                    {
                        Message = $"[Compiler] Label '{labelblock.LabelName}' is empty.",
                        Line = labelblock.Line,
                        Column = labelblock.Column,
                        Span = new TextSpan()
                        {
                            StartLine = label.LabelName.Line,
                            StartColumn = label.LabelName.Column,
                            EndLine = label.LabelName.Line,
                            EndColumn = label.LabelName.Column + label.LabelName.Lexeme.Length,
                        },
                        Severity = Diagnostic.SeverityLevel.Warning
                    });
                }
            }

            ImportedLabelSets[sourceId] = labelSet;
            SymbolTables.UpdateFileSymbols(table);

            if (diagnostics?.Counts[Diagnostic.SeverityLevel.Error] > 0)
            {
                Diagnostics.Report(new Diagnostic
                {
                    Message = $"[Compiler] Compilation of '{sourceId}' failed with {diagnostics.Counts[Diagnostic.SeverityLevel.Error]} error(s).",
                    Line = line,
                    Column = column,
                    Severity = Diagnostic.SeverityLevel.Warning
                });
            }
        }

        public async Task<CompileResult> CompileAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var code = await GetSourceTextAsync(SourceID, cancellationToken).ConfigureAwait(false);
                await CompileInternalAsync(SourceID, code, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load source from '{SourceID}': {ex.Message}", ex);
            }

            // ======================== Semantic checking ========================
            var table = SymbolTables.GetFileSymbolTable(SourceID);
            // Duplicate References
            foreach (var reference in table.References)
            {
                if (reference.Value.Count > 1)
                {
                    Diagnostics.Report(new Diagnostic
                    {
                        Message = $"[Compiler] Duplicate import of '{reference.Key}'.",
                        Line = reference.Value[0].Line,
                        Column = reference.Value[0].Column,
                        Span = new TextSpan()
                        {
                            StartLine = reference.Value[0].Line,
                            StartColumn = reference.Value[0].Column,
                            EndLine = reference.Value[0].Line,
                            EndColumn = reference.Value[0].Column + reference.Key.Length,
                        },
                        Severity = Diagnostic.SeverityLevel.Warning
                    });
                }
            }
            // Check label
            foreach (var usage in table.LabelUsages)
            {
                var defpos = SymbolTables.FindLabelDefinition(SourceID, usage.Key);
                if (defpos.Count == 0)
                {
                    foreach (var pos in usage.Value)
                    {
                        Diagnostics.Report(new Diagnostic
                        {
                            Message = $"[Compiler] Undefined label '{usage.Key}'.",
                            Line = pos.Line,
                            Column = pos.Column,
                            Span = new TextSpan()
                            {
                                StartLine = pos.Line,
                                StartColumn = pos.Column,
                                EndLine = pos.Line,
                                EndColumn = pos.Column + usage.Key.Length,
                            },
                            Severity = Diagnostic.SeverityLevel.Error
                        });
                    }
                }
                else if (defpos.Count > 1)
                {
                    foreach (var pos in defpos)
                    {
                        if (pos.SourceID == SourceID)
                        {
                            Diagnostics.Report(new Diagnostic
                            {
                                Message = $"[Compiler] Duplicate label definition: '{usage.Key}'.",
                                Line = pos.Line,
                                Column = pos.Column,
                                Span = new TextSpan()
                                {
                                    StartLine = pos.Line,
                                    StartColumn = pos.Column,
                                    EndLine = pos.Line,
                                    EndColumn = pos.Column + usage.Key.Length,
                                },
                                Severity = Diagnostic.SeverityLevel.Error
                            });
                        }
                        else
                        {
                            Diagnostics.Report(new Diagnostic
                            {
                                Message = $"[Compiler] Duplicate label definition '{usage.Key}' in file.",
                                Line = table.References[pos.SourceID][0].Line,
                                Column = table.References[pos.SourceID][0].Column,
                                Span = new TextSpan()
                                {
                                    StartLine = table.References[pos.SourceID][0].Line,
                                    StartColumn = table.References[pos.SourceID][0].Column,
                                    EndLine = table.References[pos.SourceID][0].Line,
                                    EndColumn = table.References[pos.SourceID][0].Column + pos.SourceID.Length,
                                },
                                Severity = Diagnostic.SeverityLevel.Error
                            });
                        }
                    }
                }
            }
            // Check variable
            foreach (var usage in table.VariableUsages)
            {
                if (usage.Key.StartsWith("global."))
                {
                    continue; // Skip .global variables
                }
                var defpos = SymbolTables.FindVariableDefinition(SourceID, usage.Key);
                if (defpos.Count == 0)
                {
                    foreach (var pos in usage.Value)
                    {
                        Diagnostics.Report(new Diagnostic
                        {
                            Message = $"[Compiler] Undefined variable '{usage.Key}'.",
                            Line = pos.Line,
                            Column = pos.Column,
                            Span = new TextSpan()
                            {
                                StartLine = pos.Line,
                                StartColumn = pos.Column,
                                EndLine = pos.Line,
                                EndColumn = pos.Column + usage.Key.Length,
                            },
                            Severity = Diagnostic.SeverityLevel.Error
                        });
                    }
                }
            }
            // ======================== End of Semantic checking ========================

            // Concat result LabelSet
            var labels = CollectLabels();
            return new CompileResult
            {
                Success = Diagnostics.Counts[Diagnostic.SeverityLevel.Error] == 0,
                Diagnostics = Diagnostics.GetAll(),
                Labels = labels,
                SourceID = SourceID,
                Timestamp = Timestamp,
            };
        }
    }
}