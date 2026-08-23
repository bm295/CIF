using CIF.Domain.CustomerProfiles;

namespace CIF.Domain.Tests.CustomerProfiles;

public sealed class GovernmentIdTests
{
    [Fact]
    public void Create_NormalizesAnUnexpiredGovernmentId()
    {
        var currentDate = new DateOnly(2026, 8, 23);
        var result = GovernmentId.Create(
            GovernmentIdType.Passport,
            " AB 123 ",
            " vn ",
            currentDate.AddDays(1),
            currentDate);

        Assert.True(result.IsSuccess);
        Assert.Equal("AB123", result.Value!.Number);
        Assert.Equal("VN", result.Value.IssuingCountryCode);
    }

    [Fact]
    public void Create_RejectsAnExpiredGovernmentId()
    {
        var currentDate = new DateOnly(2026, 8, 23);
        var result = GovernmentId.Create(
            GovernmentIdType.Passport,
            "AB123",
            "VN",
            currentDate.AddDays(-1),
            currentDate);

        var error = Assert.Single(result.Errors);
        Assert.Equal("Expired", error.Code);
        Assert.Equal("governmentId.expiryDate", error.Field);
    }
}
