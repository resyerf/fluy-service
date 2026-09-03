using FluentValidation;

namespace Fluy.Application.Commands.Documents.UploadDocument;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    private const long MaxSizeBytes = 25 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".pdf", ".xlsx", ".xls", ".png", ".jpg", ".jpeg", ".docx", ".doc"];

    public UploadDocumentCommandValidator()
    {
        RuleFor(c => c.RequestId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty().MaximumLength(255)
            .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage($"Extensión no permitida. Formatos aceptados: {string.Join(", ", AllowedExtensions)}.");
        RuleFor(c => c.SizeBytes).GreaterThan(0).LessThanOrEqualTo(MaxSizeBytes)
            .WithMessage("El archivo no puede superar los 25 MB.");
    }
}
