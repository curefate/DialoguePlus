using Narratoria.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var lexer = new Lexer("../../../TestScripts/text.narr");
        foreach (var token in lexer.Tokenize())
        {
            Console.WriteLine(token);
        }
    }
}