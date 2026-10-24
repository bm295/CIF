using CIF.Application.CustomerProfiles.Offers;
using CIF.Infrastructure.Offers;

namespace CIF.Application.Tests.CustomerProfiles;

public sealed class OfferAcceptanceScorerTests
{
    [Fact]
    public void BundledModelMatchesPythonExport()
    {
        var scorer = new JsonOfferAcceptanceScorer(Path.Combine(AppContext.BaseDirectory, "Models", "offer-acceptance.json"));
        Assert.Equal(0.3113115231487099, scorer.Score(new(250000m, 100000m, 50000m)), 10);
    }
}
