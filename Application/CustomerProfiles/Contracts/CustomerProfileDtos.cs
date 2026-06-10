using CIF.Domain.CustomerProfiles;

namespace CIF.Application.CustomerProfiles.Contracts;

public sealed record UpsertCustomerProfileRequest(
    string? FullName,
    DateOnly? DateOfBirth,
    string? PhoneNumber,
    string? Email,
    AddressDto? Address,
    GovernmentIdDto? GovernmentId,
    string Actor);

public sealed record CustomerProfileResponse(
    Guid CustomerId,
    string FullName,
    DateOnly? DateOfBirth,
    string? PhoneNumber,
    string? Email,
    AddressDto Address,
    GovernmentIdDto GovernmentId,
    KycStatus KycStatus,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    string CreatedBy,
    DateTimeOffset UpdatedAtUtc,
    string UpdatedBy);

public sealed record CustomerSearchQuery(
    string? Keyword,
    KycStatus? KycStatus,
    int Page = 1,
    int PageSize = 20,
    bool IncludeInactive = false);

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount);
