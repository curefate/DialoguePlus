using Narratoria.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var lexer = new Lexer("../../../TestScripts/text.narr");
        var tokens = new List<Token>(lexer.Tokenize());
        Console.WriteLine("========================== Tokens ==========================");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
        var parser = new Parser(tokens);
        var program = parser.Parse();
        Console.WriteLine("======================== AST =====================");
        foreach (var import in program.Imports)
        {
            Console.WriteLine($"Import: {import.Path.Text}");
        }
        foreach (var stmt in program.TopLevelStatements)
        {
            Console.WriteLine($"Top Level Statement: {stmt}");
        }
        foreach (var label in program.Labels)
        {
            Console.WriteLine($"Label: {label.LabelName.Text}, Statements: {label.Statements.Count}");
        }
    }
}