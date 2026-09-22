using CIF.Application.CustomerProfiles.Offers;

namespace CIF.Application.Tests.CustomerProfiles;

public sealed class OfferRecommendationServiceTests
{
    [Fact]
    public void RecommendsHighestPairWithinBudget()
    {
        var result = new OfferRecommendationService().Recommend(new(
            250m,
            [new("basic", "Basic", 100m), new("premium", "Premium", 200m)],
            [new("support", "Support", 50m), new("report", "Report", 75m)]));

        Assert.NotNull(result);
        Assert.Equal(250m, result!.Total);
        Assert.Equal("premium", result.BasePlan.Id);
        Assert.Equal("support", result.AddOn.Id);
    }

    [Fact]
    public void ReturnsNullWhenNoPairFits()
    {
        var result = new OfferRecommendationService().Recommend(new(
            10m, [new("premium", "Premium", 200m)], [new("support", "Support", 50m)]));

        Assert.Null(result);
    }
}
