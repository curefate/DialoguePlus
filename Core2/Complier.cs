namespace Narratoria.Core
{
    public class Compiler : BaseDiagnosticReporter
    {
        private readonly string _extension = ".narr";
        private readonly SymbolTableManager _tableManager;
        public SymbolTableManager TableManager => _tableManager;

        public Compiler(SymbolTableManager? tableManager = null)
        {
            _tableManager = tableManager ?? new SymbolTableManager();
        }

        private SIRSet _Compile(string filePath, HashSet<string> importedFiles, bool isRoot = false)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            filePath = Path.GetFullPath(filePath);
            importedFiles.Add(filePath);
            // Lexing and Parsing
            var lexer = new Lexer(filePath);
            foreach (var listener in _listeners)
            {
                lexer.AttachDiagnosticListener(listener);
            }
            var tokens = new List<Token>(lexer.Tokenize());
            var parser = new Parser(tokens, filePath);
            foreach (var listener in _listeners)
            {
                parser.AttachDiagnosticListener(listener);
            }
            var ast = parser.Parse();

            var sirSet = new SIRSet
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                SourceFile = filePath,
            };
            var table = new FileSymbolTable
            {
                FilePath = filePath,
            };
            IRBuilder builder = new(table);
            foreach (var listener in _listeners)
            {
                builder.AttachDiagnosticListener(listener);
            }

            // Handle imports
            foreach (var import in ast.Imports)
            {
                var path = import.Path.Lexeme;
                path = Path.IsPathFullyQualified(path) ? path : Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, path));
                if (string.IsNullOrEmpty(path))
                {
                    Report(new Diagnostic
                    {
                        Message = "[Compiler] Import path cannot be empty.",
                        Line = import.Path.Line,
                        Column = import.Path.Column,
                        Span = new TextSpan
                        {
                            StartLine = import.Path.Line,
                            StartColumn = import.Path.Column,
                            EndLine = import.Path.Line,
                            EndColumn = import.Path.Column + import.Path.Lexeme.Length,
                        },
                        Severity = Diagnostic.SeverityLevel.Error,
                    });
                    continue;
                }
                if (!File.Exists(path))
                {
                    Report(new Diagnostic
                    {
                        Message = $"[Compiler] Imported file not found: {path}",
                        Line = import.Path.Line,
                        Column = import.Path.Column,
                        Span = new TextSpan
                        {
                            StartLine = import.Path.Line,
                            StartColumn = import.Path.Column,
                            EndLine = import.Path.Line,
                            EndColumn = import.Path.Column + import.Path.Lexeme.Length,
                        },
                        Severity = Diagnostic.SeverityLevel.Error,
                    });
                    continue;
                }
                if (!string.Equals(Path.GetExtension(path), _extension, StringComparison.OrdinalIgnoreCase))
                {
                    Report(new Diagnostic
                    {
                        Message = $"[Compiler] Imported file must have '{_extension}' extension: {path}",
                        Line = import.Path.Line,
                        Column = import.Path.Column,
                        Span = new TextSpan
                        {
                            StartLine = import.Path.Line,
                            StartColumn = import.Path.Column,
                            EndLine = import.Path.Line,
                            EndColumn = import.Path.Column + import.Path.Lexeme.Length,
                        },
                        Severity = Diagnostic.SeverityLevel.Error,
                    });
                    continue;
                }
                if (importedFiles.Contains(path))
                {
                    Report(new Diagnostic
                    {
                        Message = $"[Compiler] Circular import detected: {path}",
                        Line = import.Path.Line,
                        Column = import.Path.Column,
                        Span = new TextSpan
                        {
                            StartLine = import.Path.Line,
                            StartColumn = import.Path.Column,
                            EndLine = import.Path.Line,
                            EndColumn = import.Path.Column + import.Path.Lexeme.Length,
                        },
                        Severity = Diagnostic.SeverityLevel.Warning,
                    });
                    continue;
                }
                var imported = _Compile(path, importedFiles, false);
                foreach (var label in imported.Labels)
                {
                    if (sirSet.Labels.ContainsKey(label.Key))
                    {
                        Report(new Diagnostic
                        {
                            Message = $"[Compiler] Duplicate label definition \"{label.Key}\" in file: {filePath}",
                            Line = label.Value.Line,
                            Column = label.Value.Column,
                            Span = new TextSpan
                            {
                                StartLine = label.Value.Line,
                                StartColumn = label.Value.Column,
                                EndLine = label.Value.Line,
                                EndColumn = label.Value.Column + label.Key.Length,
                            },
                            Severity = Diagnostic.SeverityLevel.Error,
                        });
                        continue;
                        // throw new Exception($"Duplicate label definition \"{label.Key}\" in file: {filePath}");
                    }
                    sirSet.Labels[label.Key] = label.Value;
                }
            }

            // Handle top-level statements
            if (isRoot)
            {
                SIR_Label topLevelLabel = new()
                {
                    LabelName = "__main__",
                    Source = filePath,
                    Line = -1,
                    Column = -1,
                };
                foreach (var stmt in ast.TopLevelStatements)
                {
                    var sir = builder.Visit(stmt);
                    topLevelLabel.Statements.Add(sir);
                }
                sirSet.Labels[topLevelLabel.LabelName] = topLevelLabel;
                table.LabelDefs[topLevelLabel.LabelName] = new SymbolPosition
                {
                    FilePath = filePath,
                    Line = -1,
                    Column = -1,
                };
            }

            // Handle labels
            foreach (var label in ast.Labels)
            {
                var labelBlock = (SIR_Label)builder.VisitLabelBlock(label);
                sirSet.Labels[labelBlock.LabelName] = labelBlock;
            }

            _tableManager.UpdateFileSymbols(table);
            return sirSet;
        }

        public SIRSet Compile(string filePath)
        {
            return _Compile(filePath, [], true);
        }
    }

    /// <summary>
    /// IRBuilder converts AST nodes into SIR instructions.
    /// </summary>
    internal class IRBuilder : BaseVisitor<SIR>
    {
        private readonly FileSymbolTable _table;
        private readonly ExprBuilder _exprBuilder;

        public IRBuilder(FileSymbolTable table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
            _exprBuilder = new ExprBuilder("__main__", _table);
        }

        public override void AttachDiagnosticListener(DiagnosticListener listener)
        {
            base.AttachDiagnosticListener(listener);
            _exprBuilder.AttachDiagnosticListener(listener);
        }

        public override SIR VisitLabelBlock(AST_LabelBlock context)
        {
            _exprBuilder.SetCurrentLabel(context.LabelName.Lexeme);
            _table.LabelDefs[context.LabelName.Lexeme] = new SymbolPosition
            {
                FilePath = _table.FilePath,
                Line = context.LabelName.Line,
                Column = context.LabelName.Column,
            };
            SIR_Label label = new()
            {
                LabelName = context.LabelName.Lexeme,
                Source = _table.FilePath,
                Line = context.Line,
                Column = context.Column,
            };
            foreach (var stmt in context.Statements)
            {
                var sir = Visit(stmt);
                label.Statements.Add(sir);
            }
            return label;
        }

        public override SIR VisitDialogue(AST_Dialogue context)
        {
            return new SIR_Dialogue
            {
                Line = context.Line,
                Column = context.Column,
                Speaker = context.Speaker?.Lexeme ?? string.Empty,
                Text = _exprBuilder.Visit(context.Text).Root as FStringNode ?? throw new Exception("Dialogue text must be an FStringNode."),
            };
        }

        public override SIR VisitMenu(AST_Menu context)
        {
            var menu = new SIR_Menu
            {
                Line = context.Line,
                Column = context.Column,
            };
            foreach (var item in context.Items)
            {
                menu.Options.Add(_exprBuilder.Visit(item.Text).Root as FStringNode ?? throw new Exception("Menu item text must be an FStringNode."));
                var blockStmts = item.Body.Select(stmt => Visit(stmt)).ToList();
                menu.Blocks.Add(blockStmts);
            }
            return menu;
        }

        public override SIR VisitJump(AST_Jump context)
        {
            _table.AddLabelUsage(context.TargetLabel.Lexeme, new SymbolPosition
            {
                FilePath = _table.FilePath,
                Line = context.TargetLabel.Line,
                Column = context.TargetLabel.Column,
            });
            return new SIR_Jump
            {
                Line = context.Line,
                Column = context.Column,
                TargetLabel = context.TargetLabel.Lexeme,
            };
        }

        public override SIR VisitTour(AST_Tour context)
        {
            _table.AddLabelUsage(context.TargetLabel.Lexeme, new SymbolPosition
            {
                FilePath = _table.FilePath,
                Line = context.TargetLabel.Line,
                Column = context.TargetLabel.Column,
            });
            return new SIR_Tour
            {
                Line = context.Line,
                Column = context.Column,
                TargetLabel = context.TargetLabel.Lexeme,
            };
        }

        public override SIR VisitCall(AST_Call context)
        {
            var call = new SIR_Call
            {
                Line = context.Line,
                Column = context.Column,
                FunctionName = context.FunctionName.Lexeme,
            };
            call.Arguments.AddRange(context.Arguments.Select(arg => _exprBuilder.Visit(arg)));
            return call;
        }

        public override SIR VisitAssign(AST_Assign context)
        {
            var varName = _exprBuilder.GetVariableName(context.Variable);
            _table.AddVariableUsage(varName, new SymbolPosition
            {
                FilePath = _table.FilePath,
                Line = context.Value.Line,
                Column = context.Value.Column,
            });
            if (!_table.VariableDefs.ContainsKey(varName))
            {
                _table.VariableDefs[varName] = new SymbolPosition
                {
                    FilePath = _table.FilePath,
                    Line = context.Variable.Line,
                    Column = context.Variable.Column,
                };
            }
            var variable = Expression.Variable(varName);
            var value = _exprBuilder.Visit(context.Value);
            return context.Operator.Type switch
            {
                TokenType.Assign => new SIR_Assign
                {
                    Line = context.Line,
                    Column = context.Column,
                    Expression = Expression.Assign(variable, value),
                },
                TokenType.PlusAssign => new SIR_Assign
                {
                    Line = context.Line,
                    Column = context.Column,
                    Expression = Expression.Assign(variable, Expression.Add(variable, value)),
                },
                TokenType.MinusAssign => new SIR_Assign
                {
                    Line = context.Line,
                    Column = context.Column,
                    Expression = Expression.Assign(variable, Expression.Subtract(variable, value)),
                },
                TokenType.MultiplyAssign => new SIR_Assign
                {
                    Line = context.Line,
                    Column = context.Column,
                    Expression = Expression.Assign(variable, Expression.Multiply(variable, value)),
                },
                TokenType.DivideAssign => new SIR_Assign
                {
                    Line = context.Line,
                    Column = context.Column,
                    Expression = Expression.Assign(variable, Expression.Divide(variable, value)),
                },
                TokenType.ModuloAssign => new SIR_Assign
                {
                    Line = context.Line,
                    Column = context.Column,
                    Expression = Expression.Assign(variable, Expression.Modulo(variable, value)),
                },
                TokenType.PowerAssign => new SIR_Assign
                {
                    Line = context.Line,
                    Column = context.Column,
                    Expression = Expression.Assign(variable, Expression.Power(variable, value)),
                },
                _ => throw new NotImplementedException($"Unknown assignment operator: {context.Operator.Type}"),
            };
        }

        public override SIR VisitIf(AST_If context)
        {
            var ifStmt = new SIR_If
            {
                Line = context.Line,
                Column = context.Column,
                Condition = _exprBuilder.Visit(context.Condition),
            };
            ifStmt.ThenBlock.AddRange(context.ThenBlock.Select(stmt => Visit(stmt)));
            if (context.ElseBlock != null)
            {
                ifStmt.ElseBlock.AddRange(context.ElseBlock.Select(stmt => Visit(stmt)));
            }
            return ifStmt;
        }
    }

    internal class ExprBuilder : BaseVisitor<Expression>
    {
        private string _currentLabel;
        private readonly FileSymbolTable _table;

        public ExprBuilder(string currentLabel, FileSymbolTable table)
        {
            _currentLabel = currentLabel;
            _table = table;
        }

        public void SetCurrentLabel(string label)
        {
            _currentLabel = label;
        }

        public override Expression VisitExprOr(AST_Expr_Or context)
        {
            var ret = Visit(context.Left);
            for (int i = 0; i < context.Rights.Count; i++)
            {
                var right = Visit(context.Rights[i]);
                ret = Expression.OrElse(ret, right);
            }
            return ret;
        }

        public override Expression VisitExprAnd(AST_Expr_And context)
        {
            var ret = Visit(context.Left);
            for (int i = 0; i < context.Rights.Count; i++)
            {
                var right = Visit(context.Rights[i]);
                ret = Expression.AndAlso(ret, right);
            }
            return ret;
        }

        public override Expression VisitExprEquality(AST_Expr_Equality context)
        {
            var left = Visit(context.Left);
            if (context.Operator == null || context.Right == null)
            {
                return left;
            }
            var right = Visit(context.Right);
            return context.Operator.Type switch
            {
                TokenType.Equal => Expression.Equal(left, right),
                TokenType.NotEqual => Expression.NotEqual(left, right),
                _ => throw new NotImplementedException($"Unknown equality operator: {context.Operator.Type}"),
            };
        }

        public override Expression VisitExprComparison(AST_Expr_Comparison context)
        {
            var left = Visit(context.Left);
            if (context.Operator == null || context.Right == null)
            {
                return left;
            }
            var right = Visit(context.Right);
            return context.Operator.Type switch
            {
                TokenType.Less => Expression.LessThan(left, right),
                TokenType.Greater => Expression.GreaterThan(left, right),
                TokenType.LessEqual => Expression.LessThanOrEqual(left, right),
                TokenType.GreaterEqual => Expression.GreaterThanOrEqual(left, right),
                _ => throw new NotImplementedException($"Unknown comparison operator: {context.Operator.Type}"),
            };
        }

        public override Expression VisitExprAdditive(AST_Expr_Additive context)
        {
            var ret = Visit(context.Left);
            for (int i = 0; i < context.Rights.Count; i++)
            {
                var right = Visit(context.Rights[i].Right);
                ret = context.Rights[i].Operator.Type switch
                {
                    TokenType.Plus => Expression.Add(ret, right),
                    TokenType.Minus => Expression.Subtract(ret, right),
                    _ => throw new NotImplementedException($"Unknown additive operator: {context.Rights[i].Operator.Type}"),
                };
            }
            return ret;
        }

        public override Expression VisitExprMultiplicative(AST_Expr_Multiplicative context)
        {
            var ret = Visit(context.Left);
            for (int i = 0; i < context.Rights.Count; i++)
            {
                var right = Visit(context.Rights[i].Right);
                ret = context.Rights[i].Operator.Type switch
                {
                    TokenType.Multiply => Expression.Multiply(ret, right),
                    TokenType.Divide => Expression.Divide(ret, right),
                    TokenType.Modulo => Expression.Modulo(ret, right),
                    _ => throw new NotImplementedException($"Unknown multiplicative operator: {context.Rights[i].Operator.Type}"),
                };
            }
            return ret;
        }

        public override Expression VisitExprPower(AST_Expr_Power context)
        {
            var left = Visit(context.Base);
            for (int i = 0; i < context.Exponents.Count; i++)
            {
                var right = Visit(context.Exponents[i]);
                left = Expression.Power(left, right);
            }
            return left;
        }

        public override Expression VisitExprUnary(AST_Expr_Unary context)
        {
            var primary = Visit(context.Primary);
            if (context.Operator == null)
            {
                return primary;
            }
            return context.Operator.Type switch
            {
                TokenType.Minus => Expression.Negate(primary),
                TokenType.Plus => primary,
                TokenType.Not => Expression.Not(primary),
                _ => throw new NotImplementedException($"Unknown unary operator: {context.Operator.Type}"),
            };
        }

        public override Expression VisitLiteral(AST_Literal context)
        {
            switch (context.Value.Type)
            {
                case TokenType.Number:
                    return Expression.Constant(float.Parse(context.Value.Lexeme));
                case TokenType.Boolean:
                    return Expression.Constant(bool.Parse(context.Value.Lexeme));
                case TokenType.Variable:
                    var varName = GetVariableName(context.Value);
                    _table.AddVariableUsage(varName, new SymbolPosition
                    {
                        FilePath = _table.FilePath,
                        Line = context.Value.Line,
                        Column = context.Value.Column,
                    });
                    return Expression.Variable(varName);
                default:
                    throw new NotImplementedException($"Unknown literal type: {context.Value.Type}");
            }
        }

        public string GetVariableName(Token variableToken)
        {
            return variableToken.Lexeme.Contains('.') ? variableToken.Lexeme[1..] : $"{_currentLabel}.{variableToken.Lexeme[1..]}";
        }

        public override Expression VisitFString(AST_FString context)
        {
            int embedCount = 0;
            List<string> fragments = [.. context.Fragments.Select(f =>
            {
                if (f.Type == TokenType.Fstring_Content)
                {
                    return f.Lexeme;
                }
                else if (f.Type == TokenType.PlaceHolder)
                {
                    embedCount++;
                    return FStringNode.EmbedSign;
                }
                else if (f.Type == TokenType.Fstring_Escape)
                {
                    return f.Lexeme switch
                    {
                        "\\n" => "\n",
                        "\\r" => "\r",
                        "\\t" => "\t",
                        "\\\"" => "\"",
                        "\\\\" => "\\",
                        "{{" => "{",
                        "}}" => "}",
                        _ => f.Lexeme,
                    };
                }
                throw new NotImplementedException($"Unknown fstring fragment type: {f.Type}");
            })];
            List<Expression> embed = [.. context.Embeds.Select(Visit)];
            if (embed.Count != embedCount)
            {
                throw new Exception("FString embed count mismatch.");
            }
            return Expression.FString(fragments, embed);
        }

        public override Expression VisitEmbedCall(AST_EmbedCall context)
        {
            var call = context.Call;
            return Expression.Call(call.FunctionName.Lexeme, [.. call.Arguments.Select(Visit)]);
        }

        public override Expression VisitEmbedExpr(AST_EmbedExpr context)
        {
            return Visit(context.Expression);
        }
    }
}