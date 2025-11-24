namespace Narratoria.Core
{
    public class Compiler
    {
        private readonly string _extension = ".narr";
        private readonly StmtBuilder _stmtBuilder = new();

        private readonly HashSet<string> _importedFiles = [];
        private readonly HashSet<string> _definedLabels = [];

        private SIRSet _Compile(string filePath, bool isRoot = false)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            // Normalize file path
            filePath = Path.GetFullPath(filePath);
            // Mark as imported
            _importedFiles.Add(filePath);
            // Lexing and Parsing
            var lexer = new Lexer(filePath);
            var tokens = new List<Token>(lexer.Tokenize());
            var parser = new Parser(tokens);
            var ast = parser.Parse();

            var sirSet = new SIRSet
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                SourcePath = filePath,
            };

            // Handle imports
            foreach (var import in ast.Imports)
            {
                var path = import.Path.Lexeme;
                path = Path.IsPathFullyQualified(path) ? path : Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, path));
                if (string.IsNullOrEmpty(path))
                {
                    throw new Exception("Import path cannot be empty.");
                }
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Imported file not found: {path}");
                }
                if (!string.Equals(Path.GetExtension(path), _extension, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception($"Imported file must have '{_extension}' extension: {path}");
                }
                if (_importedFiles.Contains(path))
                {
                    continue;
                }
                var importedSIRSet = _Compile(path, false);
                foreach (var label in importedSIRSet.Labels)
                {
                    if (sirSet.Labels.ContainsKey(label.Key))
                    {
                        throw new Exception($"Duplicate label definition in imports: {label.Key}");
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
                };
                _definedLabels.Add(topLevelLabel.LabelName);
                _stmtBuilder.CurrentLabel = topLevelLabel.LabelName;
                foreach (var stmt in ast.TopLevelStatements)
                {
                    var sir = _stmtBuilder.Visit(stmt);
                    topLevelLabel.Statements.Add(sir);
                }
                sirSet.Labels[topLevelLabel.LabelName] = topLevelLabel;
            }

            // Handle labels
            foreach (var label in ast.Labels)
            {
                if (_definedLabels.Contains(label.LabelName.Lexeme))
                {
                    throw new Exception($"Duplicate label definition: {label.LabelName.Lexeme}");
                }
                SIR_Label sirLabel = new()
                {
                    LabelName = label.LabelName.Lexeme,
                    Source = filePath,
                };
                _definedLabels.Add(label.LabelName.Lexeme);
                _stmtBuilder.CurrentLabel = label.LabelName.Lexeme;
                foreach (var stmt in label.Statements)
                {
                    var sir = _stmtBuilder.Visit(stmt);
                    sirLabel.Statements.Add(sir);
                }
                sirSet.Labels[sirLabel.LabelName] = sirLabel;
            }

            return sirSet;
        }

        public SIRSet Compile(string filePath)
        {
            _importedFiles.Clear();
            _definedLabels.Clear();
            return _Compile(filePath, true);
        }
    }

    internal class StmtBuilder : SyntaxBaseVisitor<SIR>
    {
        private readonly ExprBuilder _exprBuilder = new();

        private readonly HashSet<string> _definedVariables = [];
        private readonly HashSet<string> _usedLabels = [];

        public string CurrentLabel { get => _exprBuilder.CurrentLabel; set => _exprBuilder.CurrentLabel = value; }

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
            return new SIR_Jump
            {
                Line = context.Line,
                Column = context.Column,
                TargetLabel = context.TargetLabel.Lexeme,
            };
        }

        public override SIR VisitTour(AST_Tour context)
        {
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
            var value = _exprBuilder.Visit(context.Value);
            var varName = context.VariableName.Lexeme.Contains('.') ? context.VariableName.Lexeme[1..] : $"{CurrentLabel}.{context.VariableName.Lexeme[1..]}";
            var variable = Expression.Variable(varName);
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

    internal class ExprBuilder : SyntaxBaseVisitor<Expression>
    {
        private readonly HashSet<string> _definedVariables = [];

        public string CurrentLabel { get; set; } = string.Empty;

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
            return context.Value.Type switch
            {
                TokenType.Number => Expression.Constant(double.Parse(context.Value.Lexeme)),
                TokenType.Boolean => Expression.Constant(bool.Parse(context.Value.Lexeme)),
                TokenType.Variable => Expression.Variable(context.Value.Lexeme.Contains('.') ? context.Value.Lexeme[1..] : $"{CurrentLabel}.{context.Value.Lexeme[1..]}"),
                _ => throw new NotImplementedException($"Unknown literal type: {context.Value.Type}"),
            };
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