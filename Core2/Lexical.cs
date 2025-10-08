
namespace Narratoria.Core
{
    using System.Text.RegularExpressions;

    // =========================== Definition ===========================

    internal enum TokenrizeMode
    {
        Fallback = -1,
        Default,
        Fstring,
        Path,
        Embed,
    }

    internal static class LexerPatterns
    {
        private static readonly List<LexicalDefinition> pattern_default =
        [
            // Special
            new LexicalDefinition(TokenType.WS, "[ \\t]+", null, true),
            new LexicalDefinition(TokenType.Linebreak, "\\r?\\n"),
            new LexicalDefinition(TokenType.Comment, "#[^\\r\\n]*", null, true),

            // Keywords
            new LexicalDefinition(TokenType.Label, "\\blabel\\b"),
            new LexicalDefinition(TokenType.Jump, "\\bjump\\b"),
            new LexicalDefinition(TokenType.Tour, "\\btour\\b"),
            new LexicalDefinition(TokenType.Call, "\\bcall\\b"),
            new LexicalDefinition(TokenType.Import, "\\bimport\\b", TokenrizeMode.Path),
            new LexicalDefinition(TokenType.If, "\\bif\\b"),
            new LexicalDefinition(TokenType.Else, "\\belse\\b"),
            new LexicalDefinition(TokenType.Elif, "\\belif\\b"),

            // Literals
            new LexicalDefinition(TokenType.Identifier, "[a-zA-Z_][a-zA-Z0-9_]*"),
            new LexicalDefinition(TokenType.Number, "-?\\d+(\\.\\d+)?"),
            new LexicalDefinition(TokenType.Boolean, "\\b(true|false)\\b"),
            new LexicalDefinition(TokenType.Variable, "\\$[a-zA-Z_][a-zA-Z0-9_]*(\\.[a-zA-Z_][a-zA-Z0-9_]*)?"),
            // Fstring
            new LexicalDefinition(TokenType.Fstring_Quote, "\"", TokenrizeMode.Fstring),

            // Operators
            // Math
            new LexicalDefinition(TokenType.Plus, "\\+"),
            new LexicalDefinition(TokenType.Minus, "-"),
            new LexicalDefinition(TokenType.Multiply, "\\*"),
            new LexicalDefinition(TokenType.Divide, "/"),
            new LexicalDefinition(TokenType.Modulo, "%"),
            new LexicalDefinition(TokenType.Power, "\\^"),
            // Assignment
            new LexicalDefinition(TokenType.Assign, "="),
            new LexicalDefinition(TokenType.PlusAssign, "\\+="),
            new LexicalDefinition(TokenType.MinusAssign, "-="),
            new LexicalDefinition(TokenType.MultiplyAssign, "\\*="),
            new LexicalDefinition(TokenType.DivideAssign, "/="),
            new LexicalDefinition(TokenType.ModuloAssign, "%="),
            new LexicalDefinition(TokenType.PowerAssign, "\\^="),
            // Comparison & Logic
            new LexicalDefinition(TokenType.Less, "<"),
            new LexicalDefinition(TokenType.Greater, ">"),
            new LexicalDefinition(TokenType.LessEqual, "<="),
            new LexicalDefinition(TokenType.GreaterEqual, ">="),
            new LexicalDefinition(TokenType.Equal, "=="),
            new LexicalDefinition(TokenType.NotEqual, "!="),
            new LexicalDefinition(TokenType.And, "\\band\\b|&&"),
            new LexicalDefinition(TokenType.Or, "\\bor\\b|\\|\\|"),
            new LexicalDefinition(TokenType.Not, "\\bnot\\b|!"),

            // Punctuation
            new LexicalDefinition(TokenType.Comma, ","),
            new LexicalDefinition(TokenType.Colon, ":"),
            new LexicalDefinition(TokenType.LParen, "\\("),
            new LexicalDefinition(TokenType.RParen, "\\)"),
            new LexicalDefinition(TokenType.LBrace, "\\{"),
            new LexicalDefinition(TokenType.RBrace, "\\}"),
        ];

        private static readonly List<LexicalDefinition> pattern_fstring =
        [
            new LexicalDefinition(TokenType.LBrace, "\\{", TokenrizeMode.Embed),
            new LexicalDefinition(TokenType.Linebreak, "\\r?\\n", null, true),
            new LexicalDefinition(TokenType.Fstring_Quote, "\"", TokenrizeMode.Fallback),
            new LexicalDefinition(TokenType.Fstring_Escape, "\\\\[\"\\\\nrt]|\\{\\{|\\}\\}"),
            new LexicalDefinition(TokenType.Fstring_Content, "[^\"\\\\\\{\\r\\n]+"),
        ];

        private static readonly List<LexicalDefinition> pattern_path =
        [
            new LexicalDefinition(TokenType.WS, "[ \\t]+", null, true),
            new LexicalDefinition(TokenType.Path, "[^\\r\\n]+"),
            new LexicalDefinition(TokenType.Linebreak, "\\r?\\n", TokenrizeMode.Fallback),
        ];

        private static readonly List<LexicalDefinition> pattern_embed =
        [
            // Special
            new LexicalDefinition(TokenType.WS, "[ \\t]+", null, true),

            // Keywords
            new LexicalDefinition(TokenType.Call, "\\bcall\\b"),

            // Literals
            new LexicalDefinition(TokenType.Identifier, "[a-zA-Z_][a-zA-Z0-9_]*"),
            new LexicalDefinition(TokenType.Number, "-?\\d+(\\.\\d+)?"),
            new LexicalDefinition(TokenType.Boolean, "\\b(true|false)\\b"),
            new LexicalDefinition(TokenType.Variable, "\\$[a-zA-Z_][a-zA-Z0-9_]*(\\.[a-zA-Z_][a-zA-Z0-9_]*)?"),
            // Fstring
            new LexicalDefinition(TokenType.Fstring_Quote, "\"", TokenrizeMode.Fstring),

            // Operators
            // Math
            new LexicalDefinition(TokenType.Plus, "\\+"),
            new LexicalDefinition(TokenType.Minus, "-"),
            new LexicalDefinition(TokenType.Multiply, "\\*"),
            new LexicalDefinition(TokenType.Divide, "/"),
            new LexicalDefinition(TokenType.Modulo, "%"),
            new LexicalDefinition(TokenType.Power, "\\^"),
            // Comparison & Logic
            new LexicalDefinition(TokenType.Less, "<"),
            new LexicalDefinition(TokenType.Greater, ">"),
            new LexicalDefinition(TokenType.LessEqual, "<="),
            new LexicalDefinition(TokenType.GreaterEqual, ">="),
            new LexicalDefinition(TokenType.Equal, "=="),
            new LexicalDefinition(TokenType.NotEqual, "!="),
            new LexicalDefinition(TokenType.And, "\\band\\b|&&"),
            new LexicalDefinition(TokenType.Or, "\\bor\\b|\\|\\|"),
            new LexicalDefinition(TokenType.Not, "\\bnot\\b|!"),

            // Punctuation
            new LexicalDefinition(TokenType.Comma, ","),
            new LexicalDefinition(TokenType.LParen, "\\("),
            new LexicalDefinition(TokenType.RParen, "\\)"),
            new LexicalDefinition(TokenType.RBrace, "\\}", TokenrizeMode.Fallback),
        ];

        public static readonly Dictionary<TokenrizeMode, List<LexicalDefinition>> PatternsMap = new()
        {
            { TokenrizeMode.Default, pattern_default },
            { TokenrizeMode.Fstring, pattern_fstring },
            { TokenrizeMode.Path, pattern_path },
            { TokenrizeMode.Embed, pattern_embed },
        };
    }

    // ========================== Classes ===========================

    internal class LexicalDefinition
    {
        public TokenType Type { get; init; }
        public Regex Regex { get; init; }
        public TokenrizeMode? PushMode { get; init; }
        public bool Ignore { get; init; }

        public LexicalDefinition(TokenType type, string pattern, TokenrizeMode? pushMode = null, bool ignore = false)
        {
            Type = type;
            Regex = new Regex($"^{pattern}", RegexOptions.Compiled);
            PushMode = pushMode;
            Ignore = ignore;
        }
    }
}