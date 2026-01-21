// Licensed under the MIT License.
// See LICENSE file in the project root.

using System.Text;

namespace DialoguePlus.Core
{
    public class Compiler
    {
        public static string Version => "1.0.2";
        private readonly IContentResolver _resolver;

        private readonly SymbolTableManager _symbolTableManager = new();
        public SymbolTableManager SymbolTables => _symbolTableManager;

        public Compiler(IContentResolver? resolver = null)
        {
            _resolver = resolver ?? new ContentResolver().Register(new CacheContentProvider()).Register(new FileContentProvider());
        }

        private static string _PathToUri(string path)
            => new Uri(Path.GetFullPath(path)).AbsoluteUri;

        private static bool _IfPath(string pathOrUri)
            => !pathOrUri.StartsWith("file://") && !pathOrUri.StartsWith("http://") && !pathOrUri.StartsWith("https://");

        public CompileResult Compile(string pathOrUri)
        {
            var sourceID = _IfPath(pathOrUri) ? _PathToUri(pathOrUri) : pathOrUri;
            var session = new CompilationSession(sourceID, _resolver);
            var result = session.CompileAsync().GetAwaiter().GetResult();
            _symbolTableManager.Merge(session.SymbolTables);
            return result;
        }
    }

    public sealed record CompileResult
    {
        public required bool Success { get; init; }
        public required List<Diagnostic> Diagnostics { get; init; }
        public required LabelSet Labels { get; init; }
        public required string SourceID { get; init; }
        public long Timestamp { get; init; }
    }

    internal class CompilationSession
    {
        private IContentResolver _resolver;

        public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public string SourceID { get; init; }

        public DiagnosticEngine Diagnostics { get; init; } = new DiagnosticEngine();
        public SymbolTableManager SymbolTables { get; init; } = new SymbolTableManager();
        public Dictionary<string, LabelSet> ImportedLabelSets { get; init; } = [];

        public CompilationSession(string sourceID, IContentResolver resolver)
        {
            SourceID = sourceID;
            _resolver = resolver;
        }

        private bool IsAbsolutePath(string path)
            => Path.IsPathRooted(path);

        private async Task<string> GetSourceTextAsync(string uri, CancellationToken cancellationToken = default)
        {
            var context = await _resolver.GetTextAsync(uri, cancellationToken).ConfigureAwait(false);
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

        private async Task CompileInternalAsync(string uri, string code, CancellationToken cancellationToken = default, int line = -1, int column = -1)
        {
            if (ImportedLabelSets.ContainsKey(uri))
            {
                return;
            }

            var diagnostics = uri == SourceID ? Diagnostics : new DiagnosticEngine();
            var lexer = new Lexer(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(code))), diagnostics);
            var parser = new Parser(lexer, diagnostics);
            var ast = parser.Parse();

            var table = new FileSymbolTable { SourceID = uri };
            var labelSet = new LabelSet();
            ImportedLabelSets.Add(uri, labelSet); // Prevent circular import
            var builder = new IRBuilder(table);

            foreach (var import in ast.Imports)
            {
                var importUri = IsAbsolutePath(import.Path.Lexeme) ? new Uri(import.Path.Lexeme).AbsoluteUri : new Uri(new Uri(uri), import.Path.Lexeme).AbsoluteUri;
                try
                {
                    var importCode = await GetSourceTextAsync(importUri, cancellationToken).ConfigureAwait(false);
                    await CompileInternalAsync(importUri, importCode, cancellationToken, import.Path.Line, import.Path.Column).ConfigureAwait(false);
                    table.AddReference(importUri, new SymbolPosition
                    {
                        SourceID = uri,
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

            if (ast.TopLevelStatements.Count > 0 && uri == SourceID)
            {
                SIR_Label topLabel = new()
                {
                    LabelName = LabelSet.DefaultEntranceLabel,
                    SourceID = uri,
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

            ImportedLabelSets[uri] = labelSet;
            SymbolTables.UpdateFileSymbols(table);

            if (diagnostics?.Counts[Diagnostic.SeverityLevel.Error] > 0)
            {
                Diagnostics.Report(new Diagnostic
                {
                    Message = $"[Compiler] Compilation of '{uri}' failed with {diagnostics.Counts[Diagnostic.SeverityLevel.Error]} error(s).",
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