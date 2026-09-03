using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Entities;

/// <summary>
/// Adjunto de una Request (CLAUDE.md §21). Entidad independiente con RequestId como FK plano —
/// mismo patrón que RequestField: se crea/consulta vía IDocumentRepository, sin colección
/// navegable desde Request. StorageKey es opaco a este dominio (lo interpreta IDocumentStorage).
/// </summary>
public class Document : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid RequestId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public Guid UploadedByUserId { get; private set; }
    public int Version { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Document()
    {
    }

    public static Document Create(
        Guid tenantId, Guid requestId, string fileName, string contentType, long sizeBytes, string storageKey, Guid uploadedByUserId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("El nombre del archivo es obligatorio.", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("La clave de almacenamiento es obligatoria.", nameof(storageKey));
        }

        if (sizeBytes <= 0)
        {
            throw new ArgumentException("El tamaño del archivo debe ser mayor a cero.", nameof(sizeBytes));
        }

        return new Document
        {
            TenantId = tenantId,
            RequestId = requestId,
            FileName = fileName.Trim(),
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StorageKey = storageKey,
            UploadedByUserId = uploadedByUserId,
            Version = 1
        };
    }
}
