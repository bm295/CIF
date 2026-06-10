namespace CIF.Application.CustomerProfiles.Contracts;

public sealed record AddressDto(
    string? Line1,
    string? Line2,
    string? WardOrLocality,
    string? DistrictOrCity,
    string? StateOrProvince,
    string? PostalCode,
    string? CountryCode);
