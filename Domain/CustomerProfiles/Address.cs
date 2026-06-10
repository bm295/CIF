using CIF.Domain.Common;

namespace CIF.Domain.CustomerProfiles;

public sealed record Address(
    string Line1,
    string? Line2,
    string? WardOrLocality,
    string DistrictOrCity,
    string? StateOrProvince,
    string? PostalCode,
    string CountryCode)
{
    public static Result<Address> Create(
        string? line1,
        string? line2,
        string? wardOrLocality,
        string? districtOrCity,
        string? stateOrProvince,
        string? postalCode,
        string? countryCode)
    {
        List<Error> errors = [];

        if (string.IsNullOrWhiteSpace(line1))
        {
            errors.Add(new Error("Required", "address.line1", "Address line 1 is required."));
        }

        if (string.IsNullOrWhiteSpace(districtOrCity))
        {
            errors.Add(new Error("Required", "address.districtOrCity", "District or city is required."));
        }

        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Trim().Length != 2)
        {
            errors.Add(new Error("InvalidCountryCode", "address.countryCode", "Country code must be ISO-3166 alpha-2."));
        }

        if (errors.Count > 0)
        {
            return Result<Address>.Failure([.. errors]);
        }

        return Result<Address>.Success(new Address(
            line1!.Trim(),
            NormalizeOptional(line2),
            NormalizeOptional(wardOrLocality),
            districtOrCity!.Trim(),
            NormalizeOptional(stateOrProvince),
            NormalizeOptional(postalCode),
            countryCode!.Trim().ToUpperInvariant()));
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
