using System.Text;

namespace Narratoria.Core
{
    public enum TokenType
    {
        // Special
        Indent, Dedent, WS, Linebreak, Comment, PlaceHolder, Error, EOF,

        // Keywords
        Label, Jump, Tour, Call, Import, If, Else, Elif,

        // Literals
        Identifier, Number, Boolean, Variable,
        // Fstring
        Fstring_Quote, Fstring_Content, Fstring_Escape,

        // Operators
        Plus, Minus, Multiply, Divide, Modulo, Power,
        Assign, PlusAssign, MinusAssign, MultiplyAssign, DivideAssign, ModuloAssign, PowerAssign,
        Less, Greater, LessEqual, GreaterEqual, Equal, NotEqual,
        And, Or, Not,

        // Punctuation
        Comma, Colon, LParen, RParen, LBrace, RBrace,

        // Others
        Path
    }


    public class Token
    {
        public TokenType Type;
        public required string Lexeme;
        public int Line;
        public int Column;
        public override string ToString() => $"({Line},{Column})".PadRight(15) + $"[{Type}]".PadRight(25) + Lexeme;
    }
}