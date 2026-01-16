using DialoguePlus.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        var path1 = "TestScripts/text.dp";
        var path2 = "DialoguePlusSample_Unity/Assets/DPScript/s1.dp";
        var executer = new Executer();
        var compiler = new Compiler();
        var result = compiler.Compile(path2);

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