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
        private Token Expect(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Current.Type == type) return Consume();
            }
            throw new Exception($"Expected one of [{string.Join(", ", types)}] but found {Current.Type} at line {Current.Line}, column {Current.Column}.");
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
            SyntaxStatement ret;

            // 允许在Statement前缩进
            bool indentConsumed = false;
            if (Match(TokenType.Indent))
            {
                Consume();
                indentConsumed = true;
            }

            if (Match(TokenType.Identifier)) ret = ParseDialogue();
            else if (Match(TokenType.Fstring_Quote))
            {
                int lookahead = 1;
                while (Peek(lookahead).Type != TokenType.Linebreak && Peek(lookahead).Type != TokenType.EOF)
                {
                    if (Peek(lookahead).Type == TokenType.Colon)
                    {
                        return ParseMenu();
                    }
                    lookahead++;
                }
                ret = ParseDialogue();
            }
            else if (Match(TokenType.Jump)) ret = ParseJump();
            else if (Match(TokenType.Tour)) ret = ParseTour();
            else if (Match(TokenType.Call)) ret = ParseCall();
            else if (Match(TokenType.Variable)) ret = ParseAssign();
            else if (Match(TokenType.If)) ret = ParseIf();
            else throw new Exception($"Unexpected token {Current.Type} at line {Current.Line}, column {Current.Column}.");

            if (indentConsumed)
            {
                Expect(TokenType.Dedent, "Expected dedentation after statement.");
            }

            return ret;
        }

        private SyntaxDialogue ParseDialogue()
        {
            string? speaker = null;
            if (Match(TokenType.Identifier))
            {
                speaker = Consume().Text;
            }
            var text = ParseFString();
            Expect(TokenType.Linebreak, "Expected newline after dialogue.");
            return new SyntaxDialogue
            {
                Speaker = speaker,
                Text = text,
                Line = text.Line,
                Column = text.Column
            };
        }

        private SyntaxMenu ParseMenu()
        {
            var menu = new SyntaxMenu
            {
                Line = Current.Line,
                Column = Current.Column
            };

            while (Match(TokenType.Fstring_Quote))
            {
                int lookahead = 1;
                while (Peek(lookahead).Type != TokenType.Linebreak && Peek(lookahead).Type != TokenType.EOF)
                {
                    if (Peek(lookahead).Type == TokenType.Colon)
                    {
                        menu.Items.Add(ParseMenuItem());
                        break;
                    }
                    lookahead++;
                }
            }

            return menu;
        }

        private SyntaxMenuItem ParseMenuItem()
        {
            var text = ParseFString();
            Expect(TokenType.Colon, "Expected ':' after menu option.");
            Expect(TokenType.Linebreak, "Expected newline after menu option.");
            Expect(TokenType.Indent, "Expected indentation after menu option.");

            var item = new SyntaxMenuItem
            {
                Text = text,
                Line = text.Line,
                Column = text.Column
            };

            while (!Match(TokenType.Dedent, TokenType.EOF))
            {
                item.Body.Add(ParseStatement());
            }

            while (Match(TokenType.Dedent)) Consume();

            return item;
        }

        private SyntaxJump ParseJump()
        {
            var jumpToken = Expect(TokenType.Jump, "Expected 'jump' keyword.");
            var targetToken = Expect(TokenType.Identifier, "Expected target label name.");
            Expect(TokenType.Linebreak, "Expected newline after jump statement.");
            return new SyntaxJump
            {
                TargetLabel = targetToken.Text,
                Line = jumpToken.Line,
                Column = jumpToken.Column
            };
        }

        private SyntaxTour ParseTour()
        {
            var tourToken = Expect(TokenType.Tour, "Expected 'tour' keyword.");
            var targetToken = Expect(TokenType.Identifier, "Expected target label name.");
            Expect(TokenType.Linebreak, "Expected newline after tour statement.");
            return new SyntaxTour
            {
                TargetLabel = targetToken.Text,
                Line = tourToken.Line,
                Column = tourToken.Column
            };
        }

        private SyntaxCall ParseCall()
        {
            var callToken = Expect(TokenType.Call, "Expected 'call' keyword.");
            var functionNameToken = Expect(TokenType.Identifier, "Expected function name.");
            Expect(TokenType.LParen, "Expected '(' after function name.");

            var call = new SyntaxCall
            {
                FunctionName = functionNameToken.Text,
                Line = callToken.Line,
                Column = callToken.Column
            };

            if (!Match(TokenType.RParen) && !Match(TokenType.Linebreak))
            {
                do
                {
                    call.Arguments.Add(ParseExpression());
                } while (Match(TokenType.Comma) && Consume() != null);
            }

            Expect(TokenType.RParen, "Expected ')' after function arguments.");
            Expect(TokenType.Linebreak, "Expected newline after call statement.");
            return call;
        }

        private SyntaxAssign ParseAssign()
        {
            var variableToken = Expect(TokenType.Variable, "Expected variable.");
            TokenType[] assignTypes =
            [
                TokenType.Assign, TokenType.PlusAssign, TokenType.MinusAssign,
                TokenType.MultiplyAssign, TokenType.DivideAssign, TokenType.ModuloAssign, TokenType.PowerAssign
            ];
            var assignToken = Expect(assignTypes);
            var expression = ParseExpression();
            Expect(TokenType.Linebreak, "Expected newline after assignment.");
            return new SyntaxAssign
            {
                VariableName = variableToken.Text,
                Operator = assignToken.Text,
                Value = expression,
                Line = variableToken.Line,
                Column = variableToken.Column
            };
        }

        private SyntaxIf ParseIf()
        {
            var ifToken = Expect(TokenType.If, "Expected 'if' keyword.");
            var condition = ParseExpression();
            Expect(TokenType.Colon, "Expected ':' after if condition.");
            Expect(TokenType.Linebreak, "Expected newline after if header.");
            Expect(TokenType.Indent, "Expected indentation after if header.");

            var ifNode = new SyntaxIf
            {
                Condition = condition,
                Line = ifToken.Line,
                Column = ifToken.Column
            };
            var currentIfNode = ifNode;

            while (!Match(TokenType.Dedent, TokenType.EOF))
            {
                currentIfNode.ThenBlock.Add(ParseStatement());
            }

            while (Match(TokenType.Dedent)) Consume();

            // 处理 elif 块
            while (Match(TokenType.Elif))
            {
                var elifToken = Expect(TokenType.Elif, "Expected 'elif' keyword.");
                var elifCondition = ParseExpression();
                Expect(TokenType.Colon, "Expected ':' after elif condition.");
                Expect(TokenType.Linebreak, "Expected newline after elif header.");
                Expect(TokenType.Indent, "Expected indentation after elif header.");

                // 将 elif 转换为嵌套的 If 语句，添加到 ElseBlock 中
                var elifNode = new SyntaxIf
                {
                    Condition = elifCondition,
                    Line = elifToken.Line,
                    Column = elifToken.Column
                };

                while (!Match(TokenType.Dedent, TokenType.EOF))
                {
                    elifNode.ThenBlock.Add(ParseStatement());
                }

                while (Match(TokenType.Dedent)) Consume();

                currentIfNode.ElseBlock = [elifNode];
                currentIfNode = elifNode;
            }

            // 处理 else 块
            if (Match(TokenType.Else))
            {
                Expect(TokenType.Else, "Expected 'else' keyword.");
                Expect(TokenType.Colon, "Expected ':' after else.");
                Expect(TokenType.Linebreak, "Expected newline after else header.");
                Expect(TokenType.Indent, "Expected indentation after else header.");

                currentIfNode.ElseBlock = [];
                while (!Match(TokenType.Dedent, TokenType.EOF))
                {
                    currentIfNode.ElseBlock.Add(ParseStatement());
                }

                while (Match(TokenType.Dedent)) Consume();
            }

            return ifNode;
        }

        private SyntaxExpression ParseExpression()
        {
            throw new NotImplementedException();
        }

        private SyntaxBinaryExpr ParseBinaryExpr(int parentPrecedence = 0)
        {
            throw new NotImplementedException();
        }

        private SyntaxUnaryExpr ParseUnaryExpr()
        {
            var operatorToken = Expect(TokenType.Plus, TokenType.Minus, TokenType.Not);
            var operand = ParsePrimaryExpr();
            return new SyntaxUnaryExpr
            {
                Operator = operatorToken.Text,
                Operand = operand,
                Line = operatorToken.Line,
                Column = operatorToken.Column
            };
        }

        private SyntaxExpression ParsePrimaryExpr()
        {
            throw new NotImplementedException();
        }

        private SyntaxLiteral ParseLiteral()
        {
            var literalToken = Expect(TokenType.Number, TokenType.Boolean);
            return new SyntaxLiteral
            {
                Value = literalToken.Text,
                Line = literalToken.Line,
                Column = literalToken.Column
            };
        }

        private SyntaxVariable ParseVariable()
        {
            var variableToken = Expect(TokenType.Variable, "Expected variable.");
            return new SyntaxVariable
            {
                Name = variableToken.Text,
                Line = variableToken.Line,
                Column = variableToken.Column
            };
        }

        private SyntaxFString ParseFString()
        {
            Expect(TokenType.Fstring_Quote, "Expected '\"' to start f-string.");
            var fstring = new SyntaxFString
            {
                Line = Current.Line,
                Column = Current.Column
            };
            while (!Match(TokenType.Fstring_Quote, TokenType.EOF))
            {
                if (Match(TokenType.Fstring_Content))
                {
                    var contentToken = Consume();
                    fstring.Fragments.Add(contentToken.Text);
                }
                else if (Match(TokenType.Fstring_Escape))
                {
                    var escapeToken = Consume();
                    fstring.Fragments.Add(escapeToken.Text);
                }
                else if (Match(TokenType.LBrace))
                {
                    fstring.Embedded.Add(ParseEmbedCall());
                }
                else
                {
                    throw new Exception($"Unexpected token {Current.Type} in f-string at line {Current.Line}, column {Current.Column}.");
                }

            }
            Expect(TokenType.Fstring_Quote, "Expected '\"' to end f-string.");
            return fstring;
        }

        private SyntaxEmbedCall ParseEmbedCall()
        {
            Expect(TokenType.LBrace, "Expected '{' to start embedded expression.");
            var call = ParseCall();
            Expect(TokenType.RBrace, "Expected '}' to end embedded expression.");
            return new SyntaxEmbedCall
            {
                Call = call,
                Line = call.Line,
                Column = call.Column
            };
        }
    }
}
