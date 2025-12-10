namespace Narratoria.Core
{
    public class Parser : BaseDiagnosticReporter
    {
        private readonly List<Token> _tokens;
        private int _position = 0;
        public string SourceFile { get; init; } = string.Empty;

        public Parser(IEnumerable<Token> tokens, string filePath)
        {
            _tokens = [.. tokens];
            SourceFile = filePath;
        }
        public Parser(List<Token> tokens, string filePath)
        {
            _tokens = tokens;
            SourceFile = filePath;
        }
        public Parser(Lexer lexer)
        {
            _tokens = [.. lexer.Tokenize()];
            SourceFile = lexer.SourceFile;
        }

        private Token Current => _position < _tokens.Count ? _tokens[_position] : _tokens[^1];
        private Token Peek(int offset = 0) => _position + offset < _tokens.Count ? _tokens[_position + offset] : _tokens[^1];
        private Token Consume() => _tokens[_position++];
        private bool Match(TokenType type) => Current.Type == type;
        private bool Match(params TokenType[] types) => Array.Exists(types, t => t == Current.Type);
        private Token Expect(TokenType type, string message)
        {
            if (Current.Type == type) return Consume();
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception($"Expected {type} but found {Current.Type}");
            }
            else
            {
                throw new Exception(message);
            }
        }
        private Token Expect(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Current.Type == type) return Consume();
            }
            throw new Exception($"Expected one of [{string.Join(", ", types)}] but found {Current.Type}.");
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

        public AST_Program Parse() //TODO diagnostics
        {
            var program = new AST_Program();

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
                    // 记录诊断信息
                    Report(new Diagnostic
                    {
                        Message = $"[Parser] {ex.Message}",
                        Line = Current.Line,
                        Column = Current.Column,
                        Severity = Diagnostic.SeverityLevel.Error
                    });

                    // 错误恢复：跳过当前语句直到行尾或文件末尾
                    while (!Match(TokenType.EOF, TokenType.Linebreak))
                    {
                        Consume();
                    }

                    if (Match(TokenType.Linebreak)) Consume();
                    while (Match(TokenType.Dedent, TokenType.Indent)) Consume();
                }
            }

            return program;
        }

        private AST_Import ParseImport()
        {
            var importToken = Expect(TokenType.Import, "Expected 'import' keyword.");
            var pathToken = Expect(TokenType.Path, "Expected import path.");
            Expect(TokenType.Linebreak, "Expected newline after import statement.");

            return new AST_Import
            {
                Path = pathToken,
                Line = importToken.Line,
                Column = importToken.Column
            };
        }

        private AST_LabelBlock ParseLabelBlock()
        {
            var labelToken = Expect(TokenType.Label, "Expected 'label' keyword.");
            var nameToken = Expect(TokenType.Identifier, "Expected label name.");
            Expect(TokenType.Colon, "Expected ':' after label name.");
            Expect(TokenType.Linebreak, "Expected newline after label header.");
            Expect(TokenType.Indent, "Expected indentation after label header.");

            var block = new AST_LabelBlock
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

        private AST_Statement ParseStatement()
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
            else throw new Exception($"Unexpected token {Current.Type}.");
        }

        private AST_Dialogue ParseDialogue()
        {
            Token? speaker = null;
            if (Match(TokenType.Identifier))
            {
                speaker = Consume();
            }
            var text = ParseFString();
            Expect(TokenType.Linebreak, "Expected newline after dialogue.");
            return new AST_Dialogue
            {
                Speaker = speaker,
                Text = text,
                Line = text.Line,
                Column = text.Column
            };
        }

        private AST_Menu ParseMenu()
        {
            var menu = new AST_Menu
            {
                Line = Current.Line,
                Column = Current.Column
            };

            while (Match(TokenType.Fstring_Quote))
            {
                if (!HasColonInLine()) break;
                menu.Items.Add(ParseMenuItem());
            }
            return menu;
        }

        private AST_MenuItem ParseMenuItem()
        {
            var text = ParseFString();
            Expect(TokenType.Colon, "Expected ':' after menu option.");
            Expect(TokenType.Linebreak, "Expected newline after menu option.");
            Expect(TokenType.Indent, "Expected indentation after menu option.");

            var item = new AST_MenuItem
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
            return item;
        }

        private AST_Jump ParseJump()
        {
            var jumpToken = Expect(TokenType.Jump, "Expected 'jump' keyword.");
            var targetToken = Expect(TokenType.Identifier, "Expected target label name.");
            Expect(TokenType.Linebreak, "Expected newline after jump statement.");
            return new AST_Jump
            {
                TargetLabel = targetToken,
                Line = jumpToken.Line,
                Column = jumpToken.Column
            };
        }

        private AST_Tour ParseTour()
        {
            var tourToken = Expect(TokenType.Tour, "Expected 'tour' keyword.");
            var targetToken = Expect(TokenType.Identifier, "Expected target label name.");
            Expect(TokenType.Linebreak, "Expected newline after tour statement.");
            return new AST_Tour
            {
                TargetLabel = targetToken,
                Line = tourToken.Line,
                Column = tourToken.Column
            };
        }

        private AST_Call ParseCall()
        {
            var callToken = Expect(TokenType.Call, "Expected 'call' keyword.");
            var functionNameToken = Expect(TokenType.Identifier, "Expected function name.");
            Expect(TokenType.LParen, "Expected '(' after function name.");
            var call = new AST_Call
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

        private AST_Assign ParseAssign()
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
            return new AST_Assign
            {
                Variable = variableToken,
                Operator = assignToken,
                Value = value,
                Line = variableToken.Line,
                Column = variableToken.Column
            };
        }

        private AST_If ParseIf()
        {
            var ifToken = Expect(TokenType.If, "Expected 'if' keyword.");
            var condition = ParseExpression();
            Expect(TokenType.Colon, "Expected ':' after if condition.");
            Expect(TokenType.Linebreak, "Expected newline after if header.");
            Expect(TokenType.Indent, "Expected indentation after if header.");
            var ifNode = new AST_If
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
                var elifNode = new AST_If
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

        private AST_Expression ParseExpression()
        {
            return ParseOr();
        }

        private AST_Expr_Or ParseOr()
        {
            var left = ParseAnd();
            var node = new AST_Expr_Or
            {
                Left = left,
                Line = left.Line,
                Column = left.Column
            };
            while (Match(TokenType.Or))
            {
                var operatorToken = Consume();
                var right = ParseAnd();
                node.Rights.Add(right);
            }
            return node;
        }

        private AST_Expr_And ParseAnd()
        {
            var left = ParseEquality();
            var node = new AST_Expr_And
            {
                Left = left,
                Line = left.Line,
                Column = left.Column
            };
            while (Match(TokenType.And))
            {
                var operatorToken = Consume();
                var right = ParseEquality();
                node.Rights.Add(right);
            }
            return node;
        }

        private AST_Expr_Equality ParseEquality()
        {
            var left = ParseComparison();
            Token? operatorToken = null;
            AST_Expr_Comparison? right = null;
            if (Match(TokenType.Equal, TokenType.NotEqual))
            {
                operatorToken = Consume();
                right = ParseComparison();
            }
            return new AST_Expr_Equality
            {
                Left = left,
                Operator = operatorToken,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        private AST_Expr_Comparison ParseComparison()
        {
            var left = ParseAdditive();
            Token? operatorToken = null;
            AST_Expr_Additive? right = null;
            if (Match(TokenType.Less, TokenType.Greater, TokenType.LessEqual, TokenType.GreaterEqual))
            {
                operatorToken = Consume();
                right = ParseAdditive();
            }
            return new AST_Expr_Comparison
            {
                Left = left,
                Operator = operatorToken,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        private AST_Expr_Additive ParseAdditive()
        {
            var left = ParseMultiplicative();
            var node = new AST_Expr_Additive
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

        private AST_Expr_Multiplicative ParseMultiplicative()
        {
            var left = ParsePower();
            var node = new AST_Expr_Multiplicative
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

        private AST_Expr_Power ParsePower()
        {
            var baseExpr = ParseUnary();
            AST_Expr_Power node = new()
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

        private AST_Expr_Unary ParseUnary()
        {
            Token? operatorToken = null;
            if (Match(TokenType.Not, TokenType.Minus, TokenType.Plus))
            {
                operatorToken = Consume();
            }
            var primary = ParsePrimary();
            return new AST_Expr_Unary
            {
                Operator = operatorToken,
                Primary = primary,
                Line = operatorToken?.Line ?? primary.Line,
                Column = operatorToken?.Column ?? primary.Column
            };
        }

        private AST_Expr_Primary ParsePrimary()
        {
            if (Match(TokenType.Number, TokenType.Boolean, TokenType.Variable))
            {
                return ParseLiteral();
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
                    return ParseEmbedExpr();
                }
            }
            if (Match(TokenType.LParen))
            {
                Expect(TokenType.LParen, "Expected '(' to start expression.");
                var expr = ParseExpression();
                Expect(TokenType.RParen, "Expected ')' to end expression.");
                return new AST_EmbedExpr
                {
                    Expression = expr,
                    Line = expr.Line,
                    Column = expr.Column
                };
            }
            throw new Exception($"Unexpected token {Current.Type}.");
        }

        private AST_Literal ParseLiteral()
        {
            var literalToken = Expect(TokenType.Number, TokenType.Boolean, TokenType.Variable);
            return new AST_Literal
            {
                Value = literalToken,
                Line = literalToken.Line,
                Column = literalToken.Column
            };
        }

        private AST_FString ParseFString()
        {
            Expect(TokenType.Fstring_Quote, "Expected '\"' to start f-string.");
            var fstring = new AST_FString
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
                    fstring.Embeds.Add(ParseExpression());
                    fstring.Fragments.Add(
                        new Token
                        {
                            Type = TokenType.PlaceHolder,
                            Lexeme = "{expr}",
                            Line = Current.Line,
                            Column = Current.Column
                        }
                    );
                }
                else
                {
                    throw new Exception($"Unexpected token {Current.Type} in f-string.");
                }
            }
            Expect(TokenType.Fstring_Quote, "Expected '\"' to end f-string.");
            return fstring;
        }

        private AST_EmbedCall ParseEmbedCall()
        {
            Expect(TokenType.LBrace, "Expected '{' to start embedded expression.");
            var callToken = Expect(TokenType.Call, "Expected 'call' keyword.");
            var functionNameToken = Expect(TokenType.Identifier, "Expected function name.");
            Expect(TokenType.LParen, "Expected '(' after function name.");
            var call = new AST_Call
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
            return new AST_EmbedCall
            {
                Call = call,
                Line = call.Line,
                Column = call.Column
            };
        }

        private AST_EmbedExpr ParseEmbedExpr()
        {
            Expect(TokenType.LBrace, "Expected '{' to start embedded expression.");
            var expr = ParseExpression();
            Expect(TokenType.RBrace, "Expected '}' to end embedded expression.");
            return new AST_EmbedExpr
            {
                Expression = expr,
                Line = expr.Line,
                Column = expr.Column
            };
        }
    }
}
