namespace Fluy.Application.Interfaces.Services;

/// <summary>
/// Puerto de almacenamiento de documentos (CODE.md §4.18, D08 pendiente). StorageKey es opaco al
/// resto de la aplicación — solo el adapter concreto sabe interpretarlo.
/// </summary>
public interface IDocumentStorage
{
    Task<string> SaveAsync(Guid tenantId, string fileName, Stream content, CancellationToken cancellationToken);
    Task<Stream> OpenAsync(string storageKey, CancellationToken cancellationToken);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
}
