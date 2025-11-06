namespace Narratoria.Core
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _position = 0;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = new List<Token>(tokens);
        }

        private Token Current => _position < _tokens.Count ? _tokens[_position] : _tokens[^1];
        private Token Peek(int offset = 0) => _position + offset < _tokens.Count ? _tokens[_position + offset] : _tokens[^1];
        private Token Consume() => _tokens[_position++];
        private bool Match(TokenType type) => Current.Type == type;
        private bool Match(params TokenType[] types) => Array.Exists(types, t => t == Current.Type);
        private Token Expect(TokenType type, string message)
        {
            if (Current.Type == type) return Consume();
            throw new Exception($"Expected {type} but found {Current.Type} at line {Current.Line}, column {Current.Column}. {message}");
        }

        public SyntaxProgram ParseProgram()
        {
            var program = new SyntaxProgram();

            while (Match(TokenType.Import))
            {
                program.Imports.Add(ParseImport());
            }

            while (!Match(TokenType.EOF))
            {
                try
                {
                    if (Match(TokenType.Label))
                    {
                        program.Labels.Add(ParseLabelBlock());
                    }
                    else
                    {
                        program.TopLevelStatements.Add(ParseStatement());
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[Parser Error] {ex.Message}");

                    // 错误恢复：跳过直到下一个语句起始或同步点
                    while (!Match(TokenType.EOF, TokenType.Linebreak, TokenType.Label, TokenType.Jump, TokenType.Tour, TokenType.Call, TokenType.If, TokenType.Variable))
                    {
                        Consume();
                    }

                    if (Match(TokenType.Linebreak)) Consume();

                    // 向外层抛出异常以供 IDE 或日志系统捕获
                    throw;
                }
            }

            return program;
        }

        private SyntaxImport ParseImport()
        {
            var importToken = Expect(TokenType.Import, "Expected 'import' keyword.");
            var pathToken = Expect(TokenType.Path, "Expected import path.");
            Expect(TokenType.Linebreak, "Expected newline after import statement.");

            return new SyntaxImport
            {
                Path = pathToken.Text,
                Line = importToken.Line,
                Column = importToken.Column
            };
        }

        private SyntaxLabelBlock ParseLabelBlock()
        {
            var labelToken = Expect(TokenType.Label, "Expected 'label' keyword.");
            var nameToken = Expect(TokenType.Identifier, "Expected label name.");
            Expect(TokenType.Colon, "Expected ':' after label name.");
            Expect(TokenType.Linebreak, "Expected newline after label header.");
            Expect(TokenType.Indent, "Expected indentation after label header.");

            var block = new SyntaxLabelBlock
            {
                LabelName = nameToken.Text,
                Line = labelToken.Line,
                Column = labelToken.Column
            };

            while (!Match(TokenType.Dedent, TokenType.EOF))
            {
                block.Statements.Add(ParseStatement());
            }

            while (Match(TokenType.Dedent)) Consume();

            return block;
        }

        private SyntaxStatement ParseStatement()
        {
            /* if (Match(TokenType.Identifier) || Match(TokenType.Fstring_Quote)) return ParseDialogue();
            if (Match(TokenType.Jump)) return ParseJump();
            if (Match(TokenType.Tour)) return ParseTour();
            if (Match(TokenType.Call)) return ParseCall();
            if (Match(TokenType.Variable)) return ParseAssign();
            if (Match(TokenType.If)) return ParseIf(); */

            throw new Exception($"Unexpected token {Current.Type} at line {Current.Line}, column {Current.Column}.");
        }

        // TODO: Implement ParseDialogue, ParseJump, ParseTour, ParseCall, ParseAssign, ParseIf, ParseExpression, etc.
    }
}
