namespace Narratoria.Core
{
    public class ParserContext
    {
        private readonly IEnumerator<Token> _tokens;

        public Token CurrentToken { get; private set; }

        public ParserContext(IEnumerable<Token> tokens)
        {
            _tokens = tokens.GetEnumerator();
            Advance();
        }

        private void Advance()
        {
            if (_tokens.MoveNext())
            {
                CurrentToken = _tokens.Current;
            }
            else
            {
                CurrentToken = new Token() { Type = TokenType.EOF, Text = "", Line = -1, Column = -1 };
            }
        }
    }
}