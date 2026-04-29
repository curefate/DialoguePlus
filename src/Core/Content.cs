using System.Collections.Concurrent;

namespace DialoguePlus.Core
{
    /// <summary>
    /// Represents the content of a source file.
    /// </summary>
    /// <param name="Text">The text content of the source file.</param>
    public sealed record SourceContent(
        string Text
    );

    /// <summary>
    /// Interface for content providers that can load source files from different sources
    /// (file system, cache, HTTP, editor buffers, etc.).
    /// </summary>
    /// <remarks>
    /// Providers work with an opaque <c>sourceId</c> string.
    /// The host is responsible for ensuring sourceId stability and uniqueness within a workspace.
    /// </remarks>
    public interface IContentProvider
    {
        /// <summary>
        /// Determines whether this provider can handle the specified <paramref name="sourceId"/>.
        /// </summary>
        bool CanHandle(string sourceId);

        /// <summary>
        /// Checks whether a source exists for the given <paramref name="sourceId"/>.
        /// </summary>
        Task<bool> ExistsAsync(string sourceId, CancellationToken ct = default);

        /// <summary>
        /// Opens and reads the text content for the given <paramref name="sourceId"/>.
        /// </summary>
        Task<SourceContent> OpenTextAsync(string sourceId, CancellationToken ct = default);
    }

    /// <summary>
    /// Content provider that reads from the local file system.
    /// </summary>
    /// <remarks>
    /// This provider accepts <paramref name="sourceId"/> in either absolute file path form
    /// (e.g. <c>C:\\foo\\bar.dp</c>) or <c>file://</c> URI form.
    /// </remarks>
    public sealed class FileContentProvider : IContentProvider
    {
        public bool CanHandle(string sourceId)
        {
            if (string.IsNullOrWhiteSpace(sourceId)) return false;

            if (sourceId.StartsWith("file://", StringComparison.OrdinalIgnoreCase)) return true;
            return Path.IsPathRooted(sourceId);
        }

        public Task<bool> ExistsAsync(string sourceId, CancellationToken ct = default)
        {
            var path = TryGetFileSystemPath(sourceId)
                ?? throw new NotSupportedException($"FileContentProvider expects a file path or file:// URI sourceId, got '{sourceId}'.");
            return Task.FromResult(File.Exists(path));
        }

        public async Task<SourceContent> OpenTextAsync(string sourceId, CancellationToken ct = default)
        {
            var path = TryGetFileSystemPath(sourceId)
                ?? throw new NotSupportedException($"FileContentProvider expects a file path or file:// URI sourceId, got '{sourceId}'.");

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            using var fs = File.OpenRead(path);
            using var sr = new StreamReader(fs, detectEncodingFromByteOrderMarks: true);
            var text = await sr.ReadToEndAsync().ConfigureAwait(false);
            return new SourceContent(text);
        }

        private static string? TryGetFileSystemPath(string sourceId)
        {
            if (string.IsNullOrWhiteSpace(sourceId)) return null;

            if (sourceId.StartsWith("file://", StringComparison.OrdinalIgnoreCase)
                && Uri.TryCreate(sourceId, UriKind.Absolute, out var uri)
                && uri.IsFile)
            {
                return uri.LocalPath;
            }

            if (Path.IsPathRooted(sourceId))
            {
                return sourceId;
            }

            return null;
        }
    }

    /// <summary>
    /// Content provider backed by an in-memory cache keyed by sourceId.
    /// </summary>
    public sealed class CacheContentProvider : IContentProvider
    {
        private readonly ConcurrentDictionary<string, SourceContent> _cache;

        public CacheContentProvider(ConcurrentDictionary<string, SourceContent>? cache = null)
        {
            _cache = cache ?? new ConcurrentDictionary<string, SourceContent>(StringComparer.Ordinal);
        }

        public bool CanHandle(string sourceId) => _cache.ContainsKey(sourceId);

        public Task<bool> ExistsAsync(string sourceId, CancellationToken ct = default)
            => Task.FromResult(_cache.ContainsKey(sourceId));

        public Task<SourceContent> OpenTextAsync(string sourceId, CancellationToken ct = default)
            => Task.FromResult(_cache[sourceId]);

        public bool TryGetValue(string sourceId, out string text)
        {
            if (_cache.TryGetValue(sourceId, out var content))
            {
                text = content.Text;
                return true;
            }
            text = null!;
            return false;
        }

        public void AddOrUpdate(string sourceId, string text)
            => _cache.AddOrUpdate(sourceId, new SourceContent(text), (_, __) => new SourceContent(text));

        public void Remove(string sourceId)
            => _cache.TryRemove(sourceId, out _);
    }

    public interface IContentResolver
    {
        Task<bool> ExistsAsync(string sourceId, CancellationToken ct = default);
        Task<SourceContent> GetTextAsync(string sourceId, CancellationToken ct = default);
    }

    public sealed class ContentResolver : IContentResolver
    {
        private readonly List<IContentProvider> _providers = new();

        public ContentResolver Register(IContentProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public async Task<bool> ExistsAsync(string sourceId, CancellationToken ct = default)
        {
            return await GetProvider(sourceId).ExistsAsync(sourceId, ct).ConfigureAwait(false);
        }

        public async Task<SourceContent> GetTextAsync(string sourceId, CancellationToken ct = default)
        {
            return await GetProvider(sourceId).OpenTextAsync(sourceId, ct).ConfigureAwait(false);
        }

        private IContentProvider GetProvider(string sourceId)
        {
            var p = _providers.FirstOrDefault(x => x.CanHandle(sourceId));
            return p ?? throw new NotSupportedException($"No content provider can handle sourceId '{sourceId}'.");
        }
    }

    /// <summary>
    /// Resolves an import specifier within a script into another sourceId.
    /// </summary>
    public interface IImportResolver
    {
        /// <summary>
        /// Resolves an import specifier to a target sourceId.
        /// </summary>
        /// <param name="fromSourceId">The sourceId of the script that contains the import statement.</param>
        /// <param name="importSpec">The raw import specifier lexeme from the script.</param>
        string Resolve(string fromSourceId, string importSpec);
    }

    /// <summary>
    /// Default import resolver that keeps compatibility with the legacy URI/path behavior.
    /// </summary>
    /// <remarks>
    /// This resolver assumes <paramref name="fromSourceId"/> is an absolute URI when resolving relative imports.
    /// Hosts that use non-URI sourceIds should provide their own <see cref="IImportResolver"/> implementation.
    /// </remarks>
    public sealed class UriLikeImportResolver : IImportResolver
    {
        public string Resolve(string fromSourceId, string importSpec)
        {
            if (string.IsNullOrWhiteSpace(fromSourceId))
                throw new ArgumentException("fromSourceId cannot be null or empty.", nameof(fromSourceId));
            if (string.IsNullOrWhiteSpace(importSpec))
                throw new ArgumentException("importSpec cannot be null or empty.", nameof(importSpec));

            // If spec is an absolute URI, just return it.
            if (Uri.TryCreate(importSpec, UriKind.Absolute, out _))
                return importSpec;

            // Compatibility: if spec is an absolute OS path, treat it as a file path URI.
            if (Path.IsPathRooted(importSpec))
                return new Uri(Path.GetFullPath(importSpec)).AbsoluteUri;

            // Otherwise, resolve relative to the current document URI.
            if (!Uri.TryCreate(fromSourceId, UriKind.Absolute, out var baseUri))
                throw new InvalidOperationException($"Cannot resolve relative import '{importSpec}' from non-URI sourceId '{fromSourceId}'.");

            return new Uri(baseUri, importSpec).AbsoluteUri;
        }
    }

    /// <summary>
    /// Aggregates content resolution and import resolution.
    /// </summary>
    /// <remarks>
    /// This is typically persisted and reused by the host (e.g. language server) across compilations.
    /// </remarks>
    public interface IScriptResolver
    {
        IContentResolver Content { get; }
        IImportResolver Imports { get; }
    }

    public sealed class ScriptResolver : IScriptResolver
    {
        public IContentResolver Content { get; }
        public IImportResolver Imports { get; }

        public ScriptResolver(IContentResolver content, IImportResolver imports)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Imports = imports ?? throw new ArgumentNullException(nameof(imports));
        }
    }

    /// <summary>
    /// Provides preset resolver configurations.
    /// </summary>
    public static class ResolverPresets
    {
        /// <summary>
        /// Creates a resolver preset that supports file:// URIs and absolute file paths, with an in-memory cache.
        /// </summary>
        public static IScriptResolver CreateFileSystemWithCache(
            ConcurrentDictionary<string, SourceContent>? cache = null,
            IImportResolver? importResolver = null)
        {
            var content = new ContentResolver()
                .Register(new CacheContentProvider(cache))
                .Register(new FileContentProvider());

            return new ScriptResolver(content, importResolver ?? new UriLikeImportResolver());
        }
    }
}