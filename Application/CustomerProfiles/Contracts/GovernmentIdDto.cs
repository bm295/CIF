using CIF.Domain.CustomerProfiles;

namespace CIF.Application.CustomerProfiles.Contracts;

public sealed record GovernmentIdDto(
    GovernmentIdType Type,
    string? Number,
    string? IssuingCountryCode,
    DateOnly? ExpiryDate);
