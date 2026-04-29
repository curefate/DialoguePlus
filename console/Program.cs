using DialoguePlus.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        var path1 = "console/test3.dp";
        var executer = new Executor();
        var resolver = ResolverPresets.CreateFileSystemWithCache();
        var compiler = new Compiler(resolver);
        var entry = args.Length > 0 ? args[0] : path1;
        entry = Path.GetFullPath(entry);
        var result = compiler.Compile(new CompileRequest
        {
            EntrySourceId = entry
        });

        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var diag in result.Diagnostics)
        {
            Console.WriteLine(diag);
        }
        Console.ResetColor();

        if (result.Success)
        {
            executer.Prepare(result.Labels);
            while (executer.HasNext)
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