using CIF.Domain.CustomerProfiles;

namespace CIF.Application.CustomerProfiles.UseCases;

public interface ICustomerProfileRepository
{
    Task AddAsync(CustomerProfile profile, CancellationToken cancellationToken);
    Task<CustomerProfile?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CustomerProfile>> SearchAsync(CustomerProfileSearchCriteria criteria, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public sealed record CustomerProfileSearchCriteria(
    string? Keyword,
    KycStatus? KycStatus,
    int Page,
    int PageSize,
    bool IncludeInactive);
