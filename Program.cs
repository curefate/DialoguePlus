using Narratoria.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var lexer = new Lexer("../../../TestScripts/text.narr");
        Console.WriteLine("========================================================");
        Console.WriteLine("Tokens:");
        foreach (var token in lexer.Tokenize())
        {
            Console.WriteLine(token);
        }
        var parser = new Parser(lexer.Tokenize());
        var program = parser.Parse();
        Console.WriteLine("========================================================");
        Console.WriteLine("Parsed Syntax Tree:");
        Console.WriteLine(program);
        Console.Error.WriteLine("test error output");
    }
}