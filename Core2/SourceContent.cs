using System.Collections.Concurrent;
using System.Text;

namespace Narratoria.Core
{
    public sealed record SourceContent(
        string Text,
        string? ETag = null,
        DateTimeOffset? LastModified = null,
        Encoding? Encoding = null
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
            using var fs = File.OpenRead(uri.LocalPath);
            using var sr = new StreamReader(fs, detectEncodingFromByteOrderMarks: true);
            var text = await sr.ReadToEndAsync(ct);
            var info = new FileInfo(uri.LocalPath);
            return new SourceContent(
                text,
                ETag: null,
                LastModified: info.Exists ? info.LastWriteTimeUtc : null,
                Encoding: sr.CurrentEncoding
            );
        }
    }

    public interface IContentResolver
    {
        Task<bool> ExistsAsync(string sourceId, CancellationToken ct = default);
        Task<SourceContent> OpenTextAsync(string sourceId, CancellationToken ct = default);
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

        public async Task<SourceContent> OpenTextAsync(string sourceId, CancellationToken ct = default)
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