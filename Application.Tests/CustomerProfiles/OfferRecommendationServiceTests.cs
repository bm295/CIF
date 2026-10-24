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
    [Fact]
    public void ModelRanksOnlyAffordableOffers()
    {
        var scorer = new PreferCheaperPlan();
        var result = new OfferRecommendationService(scorer).Recommend(new(
            250m, [new("basic", "Basic", 100m), new("premium", "Premium", 200m), new("expensive", "X", 300m)],
            [new("support", "Support", 50m)], true));
        Assert.Equal("basic", result!.BasePlan.Id);
        Assert.Equal(.9, result.AcceptanceProbability);
        Assert.Equal(2, scorer.Calls);
    }

    [Fact]
    public void ModelIsOptInAndZeroBudgetIsFinite()
    {
        var result = new OfferRecommendationService(new PreferCheaperPlan()).Recommend(new(
            250m, [new("basic", "Basic", 100m), new("premium", "Premium", 200m)],
            [new("support", "Support", 50m)]));
        Assert.Equal("premium", result!.BasePlan.Id);
        Assert.Null(result.AcceptanceProbability);
        Assert.All(new OfferAcceptanceFeatures(0, 0, 0).ToVector(), x => Assert.True(double.IsFinite(x)));
    }

    [Fact]
    public void RejectsNegativePricesAndMissingModel()
    {
        Assert.Throws<ArgumentException>(() => new OfferRecommendationService().Recommend(new(
            100m, [new("a", "A", -1m)], [new("b", "B", 1m)])));
        Assert.Throws<InvalidOperationException>(() => new OfferRecommendationService().Recommend(new(
            100m, [new("a", "A", 1m)], [new("b", "B", 1m)], true)));
    }

    private sealed class PreferCheaperPlan : IOfferAcceptanceScorer
    {
        public int Calls { get; private set; }
        public double Score(OfferAcceptanceFeatures features)
        {
            Calls++;
            return features.PlanPrice <= 100m ? .9 : .2;
        }
    }
}
