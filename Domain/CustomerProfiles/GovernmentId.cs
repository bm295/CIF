using CIF.Domain.Common;

namespace CIF.Domain.CustomerProfiles;

public sealed record GovernmentId(
    GovernmentIdType Type,
    string Number,
    string IssuingCountryCode,
    DateOnly? ExpiryDate)
{
    public static Result<GovernmentId> Create(
        GovernmentIdType type,
        string? number,
        string? issuingCountryCode,
        DateOnly? expiryDate,
        DateOnly currentDate)
    {
        List<Error> errors = [];

        if (string.IsNullOrWhiteSpace(number))
        {
            errors.Add(new Error("Required", "governmentId.number", "Government ID number is required."));
        }

        if (string.IsNullOrWhiteSpace(issuingCountryCode) || issuingCountryCode.Trim().Length != 2)
        {
            errors.Add(new Error("InvalidCountryCode", "governmentId.issuingCountryCode", "Issuing country code must be ISO-3166 alpha-2."));
        }

        if (expiryDate is not null && expiryDate < currentDate)
        {
            errors.Add(new Error("Expired", "governmentId.expiryDate", "Government ID expiry date cannot be in the past."));
        }

        if (errors.Count > 0)
        {
            return Result<GovernmentId>.Failure([.. errors]);
        }

        return Result<GovernmentId>.Success(new GovernmentId(
            type,
            NormalizeNumber(number!),
            issuingCountryCode!.Trim().ToUpperInvariant(),
            expiryDate));
    }

    private static string NormalizeNumber(string value) => value.Trim().Replace(" ", string.Empty, StringComparison.Ordinal);
}
