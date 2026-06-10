using CIF.Application.CustomerProfiles.Contracts;
using CIF.Domain.CustomerProfiles;

namespace CIF.Application.CustomerProfiles.UseCases;

internal static class CustomerProfileMapper
{
    internal static CustomerProfileResponse ToResponse(this CustomerProfile profile) => new(
        profile.CustomerId,
        profile.FullName,
        profile.DateOfBirth,
        profile.PhoneNumber,
        profile.Email,
        new AddressDto(
            profile.Address.Line1,
            profile.Address.Line2,
            profile.Address.WardOrLocality,
            profile.Address.DistrictOrCity,
            profile.Address.StateOrProvince,
            profile.Address.PostalCode,
            profile.Address.CountryCode),
        new GovernmentIdDto(
            profile.GovernmentId.Type,
            profile.GovernmentId.Number,
            profile.GovernmentId.IssuingCountryCode,
            profile.GovernmentId.ExpiryDate),
        profile.KycStatus,
        profile.IsActive,
        profile.CreatedAtUtc,
        profile.CreatedBy,
        profile.UpdatedAtUtc,
        profile.UpdatedBy);
}
