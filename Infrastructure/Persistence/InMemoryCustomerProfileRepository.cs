using System.Collections.Concurrent;
using CIF.Application.CustomerProfiles.UseCases;
using CIF.Domain.CustomerProfiles;

namespace CIF.Infrastructure.Persistence;

public sealed class InMemoryCustomerProfileRepository : ICustomerProfileRepository
{
    private readonly ConcurrentDictionary<Guid, CustomerProfile> _profiles = new();

    public Task AddAsync(CustomerProfile profile, CancellationToken cancellationToken)
    {
        _profiles.TryAdd(profile.CustomerId, profile);
        return Task.CompletedTask;
    }

    public Task<CustomerProfile?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        _profiles.TryGetValue(customerId, out var profile);
        return Task.FromResult(profile);
    }

    public Task<IReadOnlyCollection<CustomerProfile>> SearchAsync(
        CustomerProfileSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var keyword = criteria.Keyword?.Trim();
        var query = _profiles.Values.AsEnumerable();

        if (!criteria.IncludeInactive)
        {
            query = query.Where(profile => profile.IsActive);
        }

        if (criteria.KycStatus is not null)
        {
            query = query.Where(profile => profile.KycStatus == criteria.KycStatus);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(profile =>
                Contains(profile.FullName, keyword) ||
                Contains(profile.Email, keyword) ||
                Contains(profile.PhoneNumber, keyword));
        }

        var profiles = query
            .OrderBy(profile => profile.FullName)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<CustomerProfile>>(profiles);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static bool Contains(string? source, string keyword) =>
        source?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true;
}
