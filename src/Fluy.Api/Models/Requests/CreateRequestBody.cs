namespace Fluy.Api.Models.Requests;

public record CreateRequestBody(
    string Title, string Description, decimal? Amount, IReadOnlyCollection<CreateRequestFieldDto>? Fields, Guid? BranchId);
