using System.Collections.Concurrent;

namespace DialoguePlus.Core
{
    public sealed record SourceContent(
        string Text
    );

    public interface IContentProvider
    {
        bool CanHandle(Uri uri);
        Task<bool> ExistsAsync(Uri uri, CancellationToken ct = default);
        Task<SourceContent> OpenTextAsync(Uri uri, CancellationToken ct = default);
    }

    public sealed class FileContentProvider : IContentProvider
    {
        public bool CanHandle(Uri uri) => uri.IsFile;

        public Task<bool> ExistsAsync(Uri uri, CancellationToken ct = default)
            => Task.FromResult(File.Exists(uri.LocalPath));

        public async Task<SourceContent> OpenTextAsync(Uri uri, CancellationToken ct = default)
        {
            if (!File.Exists(uri.LocalPath))
                throw new FileNotFoundException($"File not found: {uri.LocalPath}");
            using var fs = File.OpenRead(uri.LocalPath);
            using var sr = new StreamReader(fs, detectEncodingFromByteOrderMarks: true);
            var text = await sr.ReadToEndAsync(ct);
            var info = new FileInfo(uri.LocalPath);
            return new SourceContent(
                text
            );
        }
    }

    public sealed class CacheContentProvider : IContentProvider
    {
        private readonly ConcurrentDictionary<Uri, SourceContent> _cache;

        public CacheContentProvider(ConcurrentDictionary<Uri, SourceContent>? cache = null)
        {
            _cache = cache ?? new ConcurrentDictionary<Uri, SourceContent>();
        }

        public bool CanHandle(Uri uri) => _cache.ContainsKey(uri);

        public Task<bool> ExistsAsync(Uri uri, CancellationToken ct = default)
            => Task.FromResult(_cache.ContainsKey(uri));

        public Task<SourceContent> OpenTextAsync(Uri uri, CancellationToken ct = default)
            => Task.FromResult(_cache[uri]);

        public bool TryGetValue(Uri uri, out string text)
        {
            if (_cache.TryGetValue(uri, out var content))
            {
                text = content.Text;
                return true;
            }
            text = null!;
            return false;
        }

        public void AddOrUpdate(Uri uri, string text)
            => _cache.AddOrUpdate(uri, new SourceContent(text), (_, __) => new SourceContent(text));

        public void Remove(Uri uri)
            => _cache.TryRemove(uri, out _);
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
            var uri = Normalize(sourceId);
            return await GetProvider(uri).ExistsAsync(uri, ct);
        }

        public async Task<SourceContent> GetTextAsync(string sourceId, CancellationToken ct = default)
        {
            var uri = Normalize(sourceId);
            return await GetProvider(uri).OpenTextAsync(uri, ct);
        }

        private static Uri Normalize(string idOrPath)
        {
            if (Uri.TryCreate(idOrPath, UriKind.Absolute, out var asUri))
                return asUri;
            var full = Path.GetFullPath(idOrPath);
            return new Uri(full);
        }

        private IContentProvider GetProvider(Uri uri)
        {
            var p = _providers.FirstOrDefault(x => x.CanHandle(uri)) ?? throw new NotSupportedException($"No content provider for scheme '{uri.Scheme}'.");
            return p;
        }
    }
}