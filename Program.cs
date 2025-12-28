using DialoguePlus.Compilation;
using DialoguePlus.Execution;

public class Program
{
    public static void Main(string[] args)
    {
        var executer = new Executer();
        var compiler = new Compiler();
        var result = compiler.Compile("../../../TestScripts/text.narr");

        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var diag in result.Diagnostics)
        {
            Console.WriteLine(diag);
        }
        Console.ResetColor();

        if (result.Success)
        {
            executer.Execute(result.Labels);
        }
        else
        {
            Console.WriteLine("Compilation failed due to errors.");
        }
    }
}