namespace CIF.Application.CustomerProfiles.Offers;

public sealed record OfferAcceptanceFeatures(decimal Budget, decimal PlanPrice, decimal AddOnPrice)
{
    public double[] ToVector() => [(double)Budget, (double)PlanPrice, (double)AddOnPrice,
        Budget == 0 ? 0 : (double)((PlanPrice + AddOnPrice) / Budget)];
}

public interface IOfferAcceptanceScorer
{
    double Score(OfferAcceptanceFeatures features);
}
