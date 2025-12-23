namespace Narratoria.Core
{
    public class Compiler
    {
        private readonly IContentResolver _resolver;

        public Compiler(IContentResolver? resolver = null)
        {
            _resolver = resolver ?? new ContentResolver().Register(new CacheContentProvider()).Register(new FileContentProvider());
        }

        private static string _PathToUri(string path)
            => new Uri(Path.GetFullPath(path)).AbsoluteUri;


        private async Task<LabelSet> _CompileAsync(CompilationSession session)
        {

        }

        public async Task<CompileResult> CompileAsync(string uriOrPath)
        {

        }

        public CompileResult Compile(string uriOrPath)
        {
            return CompileAsync(uriOrPath).GetAwaiter().GetResult();
        }
    }

    public sealed record CompileResult
    {
        public required bool Success { get; init; }
        public required List<Diagnostic> Diagnostics { get; init; }
        public required LabelSet SirSet { get; init; }
        public required string Uri { get; init; }
        public long Timestamp { get; init; }
    }

    public class CompilationSession
    {
        private HashSet<string> _importedInSession = [];
        private IContentResolver _resolver;

        public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public required string SourceID { get; init; }

        public DiagnosticEngine Diagnostics { get; init; } = new DiagnosticEngine();
        public SymbolTableManager SymbolTables { get; init; } = new SymbolTableManager();
        public Dictionary<string, LabelSet> ImportedLabelSets { get; init; } = [];

        public CompilationSession(string sourceID, IContentResolver resolver)
        {
            SourceID = sourceID;
            _resolver = resolver;
        }

        private bool _HasImported(string uri)
            => _importedInSession.Contains(uri);

        
    }
}