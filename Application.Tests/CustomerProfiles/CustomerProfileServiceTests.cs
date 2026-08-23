using CIF.Application.Common;
using CIF.Application.CustomerProfiles.Contracts;
using CIF.Application.CustomerProfiles.UseCases;
using CIF.Domain.CustomerProfiles;

namespace CIF.Application.Tests.CustomerProfiles;

public sealed class CustomerProfileServiceTests
{
    [Fact]
    public async Task Create_UsesInjectedClockForGovernmentIdExpiryValidation()
    {
        var clock = new FrozenClock(new DateTimeOffset(2030, 6, 15, 12, 0, 0, TimeSpan.Zero));
        var service = new CustomerProfileService(new StubRepository(), clock);
        var request = new UpsertCustomerProfileRequest(
            "Ada Lovelace",
            null,
            null,
            "ada@example.test",
            new AddressDto("1 Computing Way", null, null, "London", null, null, "GB"),
            new GovernmentIdDto(GovernmentIdType.Passport, "123", "GB", new DateOnly(2030, 6, 14)),
            "test");

        var result = await service.CreateAsync(request);

        var error = Assert.Single(result.Errors);
        Assert.Equal("Expired", error.Code);
    }

    [Fact]
    public async Task Create_ReadsClockOnceForTheWholeOperation()
    {
        var clock = new CountingClock(new DateTimeOffset(2030, 6, 15, 12, 0, 0, TimeSpan.Zero));
        var service = new CustomerProfileService(new StubRepository(), clock);
        var request = new UpsertCustomerProfileRequest(
            "Ada Lovelace",
            null,
            null,
            "ada@example.test",
            new AddressDto("1 Computing Way", null, null, "London", null, null, "GB"),
            new GovernmentIdDto(GovernmentIdType.Passport, "123", "GB", new DateOnly(2030, 6, 15)),
            "test");

        var result = await service.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, clock.ReadCount);
        Assert.Equal(clock.Value, result.Value!.CreatedAtUtc);
    }

    private sealed record FrozenClock(DateTimeOffset UtcNow) : ISystemClock;

    private sealed class CountingClock(DateTimeOffset value) : ISystemClock
    {
        public DateTimeOffset Value { get; } = value;
        public int ReadCount { get; private set; }
        public DateTimeOffset UtcNow
        {
            get
            {
                ReadCount++;
                return Value;
            }
        }
    }

    private sealed class StubRepository : ICustomerProfileRepository
    {
        public Task AddAsync(CustomerProfile profile, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<CustomerProfile?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken) => Task.FromResult<CustomerProfile?>(null);
        public Task<IReadOnlyCollection<CustomerProfile>> SearchAsync(CustomerProfileSearchCriteria criteria, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<CustomerProfile>>([]);
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
