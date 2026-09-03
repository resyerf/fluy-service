namespace Fluy.Infrastructure.External.Services;

public class DocumentStorageOptions
{
    public const string SectionName = "DocumentStorage";

    public string RootPath { get; set; } = "App_Data/documents";
}
