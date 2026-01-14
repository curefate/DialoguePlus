using DialoguePlus.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var executer = new Executer();
        var compiler = new Compiler();
        var result = compiler.Compile("TestScripts/text.dp");

        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var diag in result.Diagnostics)
        {
            Console.WriteLine(diag);
        }
        Console.ResetColor();

        if (result.Success)
        {
            executer.Prepare(result.Labels);
            while(executer.HasNext)
            {
                await executer.StepAsync();
            }
        }
        else
        {
            Console.WriteLine("Compilation failed due to errors.");
        }
    }
}