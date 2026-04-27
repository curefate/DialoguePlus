namespace DialoguePlus.Core
{
    public static class CoreVersion
    {
        public static string Current =>
            typeof(CoreVersion).Assembly.GetName().Version?.ToString() ?? "0.0.0";
    }
}