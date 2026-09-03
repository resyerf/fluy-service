using Fluy.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Fluy.Infrastructure.External.Services;

/// <summary>
/// Adapter local de IDocumentStorage (CODE.md §4.18, D08 pendiente) — igual espíritu que
/// SmtpEmailSender/MailHog: funciona de punta a punta hoy sobre disco local, se reemplaza por
/// Azure Blob/S3 cuando D08 se resuelva, sin tocar ningún handler (el puerto no cambia).
/// </summary>
public class LocalDiskDocumentStorage(IOptions<DocumentStorageOptions> options) : IDocumentStorage
{
    private readonly string rootPath = options.Value.RootPath;

    public async Task<string> SaveAsync(Guid tenantId, string fileName, Stream content, CancellationToken cancellationToken)
    {
        var storageKey = $"{tenantId}/{Guid.NewGuid()}-{fileName}";
        var fullPath = Path.Combine(rootPath, storageKey);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return storageKey;
    }

    public Task<Stream> OpenAsync(string storageKey, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(rootPath, storageKey);
        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(rootPath, storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
