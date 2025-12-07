namespace Narratoria.Core
{
    using System.Text;
    using System.Text.RegularExpressions;

    public class Lexer : IDiagnosticReporter
    {
        private readonly List<DiagnosticCollector> _collectors = [];
        public void Report(Diagnostic diagnostic)
        {
            foreach (var collector in _collectors)
            {
                collector.Add(diagnostic);
            }
        }
        public void AddCollector(DiagnosticCollector collector)
        {
            _collectors.Add(collector);
        }

        // Source Stream
        private readonly StreamReader _inputStream;
        public string SourceFile { get; init; } = string.Empty;
        private int _line = 0;
        private int _column = 1;

        // Indentation Tracking
        private Stack<int> _indentStack = new();
        private int CurrentIndent => _indentStack.Count > 0 ? _indentStack.Peek() : 0;


        // Tokenrize Mode
        private readonly Stack<TokenrizeMode> _modeStack = new([TokenrizeMode.Default]);
        private TokenrizeMode CurrentMode
        {
            get
            {
                if (_modeStack.Peek() == TokenrizeMode.Fallback)
                {
                    _modeStack.Pop();
                    if (_modeStack.Count == 0)
                    {
                        throw new Exception("Lexer mode stack underflow.");
                    }
                    _modeStack.Pop();
                }
                return _modeStack.Peek();
            }
        }
        private List<LexicalDefinition> CurrentPatterns => LexerPatterns.PatternsMap[CurrentMode];

        public IEnumerable<Token> Tokenize() // TODO diagnostics
        {
            while (!_inputStream.EndOfStream)
            {
                // Read new line
                var line = _inputStream.ReadLine() ?? "";
                _line++;
                _column = 1;
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (CurrentMode == TokenrizeMode.Default && line.TrimStart().StartsWith('#')) continue; // Skip single-line comments in default mode
                line += "\n"; // Add linebreak for easier handling

                // Handle indentation
                if (CurrentMode == TokenrizeMode.Default)
                {
                    int newIndent = 0;
                    foreach (char c in line)
                    {
                        if (c == ' ') newIndent++;
                        else if (c == '\t') newIndent += 4;
                        else break;
                    }
                    newIndent /= 4;
                    if (newIndent > CurrentIndent)
                    {
                        _indentStack.Push(newIndent);
                        yield return new Token
                        {
                            Type = TokenType.Indent,
                            Lexeme = "",
                            Line = _line,
                            Column = _column
                        };
                    }
                    else if (newIndent < CurrentIndent)
                    {
                        while (newIndent < CurrentIndent)
                        {
                            _indentStack.Pop();
                            yield return new Token
                            {
                                Type = TokenType.Dedent,
                                Lexeme = "",
                                Line = _line,
                                Column = _column
                            };
                        }
                        if (newIndent != CurrentIndent)
                        {
                            throw new Exception($"Inconsistent indentation at line {_line}.");
                        }
                    }
                }

                // Tokenize the line
                var errorBuffer = new StringBuilder();
                while (line.Length > 0)
                {
                    Match? match = null;
                    foreach (var pattern in CurrentPatterns)
                    {
                        match = pattern.Regex.Match(line);
                        if (match.Success && match.Index == 0)
                        {
                            // If there is any error buffer, flush it as an error token first
                            if (errorBuffer.Length > 0)
                            {
                                yield return new Token
                                {
                                    Type = TokenType.Error,
                                    Lexeme = errorBuffer.ToString(),
                                    Line = _line,
                                    Column = _column - errorBuffer.Length
                                };
                                errorBuffer.Clear();
                            }

                            // Handle the matched token
                            if (pattern.PushMode != null)
                            {
                                _modeStack.Push(pattern.PushMode.Value);
                            }
                            if (!pattern.Ignore)
                            {
                                yield return new Token
                                {
                                    Type = pattern.Type,
                                    Lexeme = match.Value,
                                    Line = _line,
                                    Column = _column
                                };
                            }

                            _column += match.Length;
                            line = line[match.Length..];
                            break;
                        }
                    }

                    // If no patterns matched, add 1st char into error buffer and continue
                    if (match == null || !match.Success)
                    {
                        errorBuffer.Append(line[0]);
                        line = line[1..];
                        _column++;
                    }
                }
                // If there is any error char at the end of the line
                if (errorBuffer.Length > 0)
                {
                    yield return new Token
                    {
                        Type = TokenType.Error,
                        Lexeme = errorBuffer.ToString(),
                        Line = _line,
                        Column = _column - errorBuffer.Length
                    };
                    errorBuffer.Clear();
                }
            }

            if (CurrentMode == TokenrizeMode.Default)
            {
                // Emit remaining dedents
                while (_indentStack.Count > 0)
                {
                    _indentStack.Pop();
                    yield return new Token
                    {
                        Type = TokenType.Dedent,
                        Lexeme = "",
                        Line = _line,
                        Column = _column
                    };
                }
            }

            yield return new Token
            {
                Type = TokenType.EOF,
                Lexeme = "",
                Line = _line,
                Column = _column
            };
        }

        public Lexer(string filePath)
        {
            filePath = Path.GetFullPath(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            this._inputStream = new StreamReader(fileStream);
            SourceFile = filePath;
        }
    }
}