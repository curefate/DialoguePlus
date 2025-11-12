namespace Narratoria.Core
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _position = 0;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = [.. tokens];
        }

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        private Token Current => _position < _tokens.Count ? _tokens[_position] : _tokens[^1];
        private Token Peek(int offset = 0) => _position + offset < _tokens.Count ? _tokens[_position + offset] : _tokens[^1];
        private Token Consume() => _tokens[_position++];
        private bool Match(TokenType type) => Current.Type == type;
        private bool Match(params TokenType[] types) => Array.Exists(types, t => t == Current.Type);
        private Token Expect(TokenType type, string message)
        {
            if (Current.Type == type) return Consume();
            if (String.IsNullOrEmpty(message))
            {
                throw new Exception($"Expected {type} but found {Current.Type}. [Ln {Current.Line}, Col {Current.Column}]");
            }
            else
            {
                throw new Exception(message + $" [Ln {Current.Line}, Col {Current.Column}]");
            }
        }
        private Token Expect(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Current.Type == type) return Consume();
            }
            throw new Exception($"Expected one of [{string.Join(", ", types)}] but found {Current.Type}. [Ln {Current.Line}, Col {Current.Column}]");
        }
        private bool HasColonInLine()
        {
            int lookahead = 0;
            while (true)
            {
                var token = Peek(lookahead);
                if (token.Type == TokenType.Colon) return true;
                if (token.Type == TokenType.Linebreak || token.Type == TokenType.EOF) return false;
                lookahead++;
            }
        }

        public SyntaxRoot Parse()
        {
            var program = new SyntaxRoot();

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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"[Parser Error] {ex.Message}");
                    Console.ResetColor();

                    // 错误恢复：跳过当前语句直到行尾或文件末尾
                    while (!Match(TokenType.EOF, TokenType.Linebreak))
                    {
                        Consume();
                    }

                    if (Match(TokenType.Linebreak)) Consume();
                    while (Match(TokenType.Dedent, TokenType.Indent)) Consume();

                    // 向外层抛出异常以供 IDE 或日志系统捕获
                    // throw;
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
                Path = pathToken,
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
                LabelName = nameToken,
                Line = labelToken.Line,
                Column = labelToken.Column
            };

            while (!Match(TokenType.Dedent, TokenType.EOF))
            {
                block.Statements.Add(ParseStatement());
            }

            Expect(TokenType.Dedent, "Expected dedentation after label block.");

            return block;
        }

        private SyntaxStatement ParseStatement()
        {

            if (Match(TokenType.Identifier)) return ParseDialogue();
            else if (Match(TokenType.Fstring_Quote))
            {
                if (HasColonInLine()) return ParseMenu();
                else return ParseDialogue();
            }
            else if (Match(TokenType.Jump)) return ParseJump();
            else if (Match(TokenType.Tour)) return ParseTour();
            else if (Match(TokenType.Call)) return ParseCall();
            else if (Match(TokenType.Variable)) return ParseAssign();
            else if (Match(TokenType.If)) return ParseIf();
            else throw new Exception($"Unexpected token {Current.Type}. [Ln {Current.Line}, Col {Current.Column}]");
        }

        private SyntaxDialogue ParseDialogue()
        {
            Token? speaker = null;
            if (Match(TokenType.Identifier))
            {
                speaker = Consume();
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
                if (!HasColonInLine()) break;
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

                Expect(TokenType.Dedent, "Expected dedentation after menu option.");
                menu.Items.Add(item);
            }

            return menu;
        }

        private SyntaxJump ParseJump()
        {
            var jumpToken = Expect(TokenType.Jump, "Expected 'jump' keyword.");
            var targetToken = Expect(TokenType.Identifier, "Expected target label name.");
            Expect(TokenType.Linebreak, "Expected newline after jump statement.");
            return new SyntaxJump
            {
                TargetLabel = targetToken,
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
                TargetLabel = targetToken,
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
                FunctionName = functionNameToken,
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
            var value = ParseExpression();
            Expect(TokenType.Linebreak, "Expected newline after assignment.");
            return new SyntaxAssign
            {
                VariableName = variableToken,
                Operator = assignToken,
                Value = value,
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

            Expect(TokenType.Dedent, "Expected dedentation after if block.");

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

                Expect(TokenType.Dedent, "Expected dedentation after elif block.");

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

                Expect(TokenType.Dedent, "Expected dedentation after else block.");
            }

            return ifNode;
        }

        private SyntaxExprOr ParseExpression()
        {
            var left = ParseAnd();
            var node = new SyntaxExprOr
            {
                Left = left,
                Line = left.Line,
                Column = left.Column
            };

            while (Match(TokenType.Or))
            {
                var operatorToken = Consume();
                var right = ParseAnd();
                node.Rights.Add((operatorToken, right));
            }

            return node;
        }

        private SyntaxExprAnd ParseAnd()
        {
            var left = ParseEquality();
            var node = new SyntaxExprAnd
            {
                Left = left,
                Line = left.Line,
                Column = left.Column
            };

            while (Match(TokenType.And))
            {
                var operatorToken = Consume();
                var right = ParseEquality();
                node.Rights.Add((operatorToken, right));
            }

            return node;
        }

        private SyntaxExprEquality ParseEquality()
        {
            var left = ParseComparison();
            if (Match(TokenType.Equal, TokenType.NotEqual))
            {
                var operatorToken = Consume();
                var right = ParseComparison();
                return new SyntaxExprEquality
                {
                    Left = left,
                    Operator = operatorToken,
                    Right = right,
                    Line = left.Line,
                    Column = left.Column
                };
            }
            else
            {
                return new SyntaxExprEquality
                {
                    Left = left,
                    Operator = null!,
                    Right = null!,
                    Line = left.Line,
                    Column = left.Column
                };
            }
        }

        private SyntaxExprComparison ParseComparison()
        {
            var left = ParseAdditive();
            if (Match(TokenType.Less, TokenType.Greater, TokenType.LessEqual, TokenType.GreaterEqual))
            {
                var operatorToken = Consume();
                var right = ParseAdditive();
                return new SyntaxExprComparison
                {
                    Left = left,
                    Operator = operatorToken,
                    Right = right,
                    Line = left.Line,
                    Column = left.Column
                };
            }
            else
            {
                return new SyntaxExprComparison
                {
                    Left = left,
                    Operator = null!,
                    Right = null!,
                    Line = left.Line,
                    Column = left.Column
                };
            }
        }

        private SyntaxExprAdditive ParseAdditive()
        {
            var left = ParseMultiplicative();
            var node = new SyntaxExprAdditive
            {
                Left = left,
                Line = left.Line,
                Column = left.Column
            };

            while (Match(TokenType.Plus, TokenType.Minus))
            {
                var operatorToken = Consume();
                var right = ParseMultiplicative();
                node.Rights.Add((operatorToken, right));
            }

            return node;
        }

        private SyntaxExprMultiplicative ParseMultiplicative()
        {
            var left = ParsePower();
            var node = new SyntaxExprMultiplicative
            {
                Left = left,
                Line = left.Line,
                Column = left.Column
            };

            while (Match(TokenType.Multiply, TokenType.Divide, TokenType.Modulo))
            {
                var operatorToken = Consume();
                var right = ParsePower();
                node.Rights.Add((operatorToken, right));
            }

            return node;
        }

        private SyntaxExprPower ParsePower()
        {
            var baseExpr = ParseUnary();
            SyntaxExprPower node = new()
            {
                Base = baseExpr,
                Line = baseExpr.Line,
                Column = baseExpr.Column
            };
            while (Match(TokenType.Power))
            {
                Expect(TokenType.Power, "Expected '^' operator.");
                var exponent = ParseUnary();
                node.Exponents.Add(exponent);
            }
            return node;
        }

        private SyntaxExprUnary ParseUnary()
        {
            Token? operatorToken = null;
            if (Match(TokenType.Not, TokenType.Minus, TokenType.Plus))
            {
                operatorToken = Consume();
            }
            var primary = ParsePrimary();
            return new SyntaxExprUnary
            {
                Operator = operatorToken,
                Primary = primary,
                Line = operatorToken?.Line ?? primary.Line,
                Column = operatorToken?.Column ?? primary.Column
            };
        }

        private SyntaxExprPrimary ParsePrimary()
        {
            if (Match(TokenType.Number, TokenType.Boolean, TokenType.Variable))
            {
                var literalToken = Consume();
                return new SyntaxLiteral
                {
                    Value = literalToken,
                    Line = literalToken.Line,
                    Column = literalToken.Column
                };
            }
            if (Match(TokenType.Fstring_Quote))
            {
                return ParseFString();
            }
            if (Match(TokenType.LBrace))
            {
                if (Peek(1).Type == TokenType.Call)
                {
                    return ParseEmbedCall();
                }
                else
                {
                    Expect(TokenType.LBrace, "Expected '{' to start embedded expression.");
                    var expr = ParseExpression();
                    Expect(TokenType.RBrace, "Expected '}' to end embedded expression.");
                    return new SyntaxEmbedExpr
                    {
                        Expression = expr,
                        Line = expr.Line,
                        Column = expr.Column
                    };
                }
            }
            if (Match(TokenType.LParen))
            {
                Expect(TokenType.LParen, "Expected '(' to start expression.");
                var expr = ParseExpression();
                Expect(TokenType.RParen, "Expected ')' to end expression.");
                return new SyntaxEmbedExpr
                {
                    Expression = expr,
                    Line = expr.Line,
                    Column = expr.Column
                };
            }
            throw new Exception($"Unexpected token {Current.Type} at line {Current.Line}, column {Current.Column}.");
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
                if (Match(TokenType.Fstring_Content, TokenType.Fstring_Escape))
                {
                    var strToken = Consume();
                    fstring.Fragments.Add(strToken);
                }
                else if (Match(TokenType.LBrace))
                {
                    fstring.Embedded.Enqueue((ParseExpression(), fstring.Fragments.Count));
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
            var callToken = Expect(TokenType.Call, "Expected 'call' keyword.");
            var functionNameToken = Expect(TokenType.Identifier, "Expected function name.");
            Expect(TokenType.LParen, "Expected '(' after function name.");
            var call = new SyntaxCall
            {
                FunctionName = functionNameToken,
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
