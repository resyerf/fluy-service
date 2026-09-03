using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IBranchRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    void Add(Branch branch);
    Task<IReadOnlyCollection<AllBranchDetail>> GetAllWithCompanyAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<BranchDetail>> GetByCompanyAsync(Guid companyId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MyBranchSummary>> GetAllSummariesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MyBranchSummary>> GetSummariesByIdsAsync(IReadOnlyCollection<Guid> branchIds, CancellationToken cancellationToken);
}
