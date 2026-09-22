namespace CIF.Application.CustomerProfiles.Offers;

public sealed record OfferOption(string Id, string Name, decimal Price);

public sealed record OfferRecommendationRequest(
    decimal Budget,
    IReadOnlyCollection<OfferOption> BasePlans,
    IReadOnlyCollection<OfferOption> AddOns);

public sealed record OfferRecommendationResponse(
    OfferOption BasePlan,
    OfferOption AddOn,
    decimal Total,
    decimal RemainingBudget);

public sealed class OfferRecommendationService
{
    public OfferRecommendationResponse? Recommend(OfferRecommendationRequest request)
    {
        if (request.Budget < 0)
            throw new ArgumentOutOfRangeException(nameof(request.Budget));

        OfferRecommendationResponse? best = null;

        // Same strategy as getMoneySpent: inspect every pair and retain
        // the highest total that remains within the customer's budget.
        foreach (var plan in request.BasePlans)
        {
            foreach (var addOn in request.AddOns)
            {
                var total = plan.Price + addOn.Price;
                if (total > request.Budget)
                    continue;

                if (best is null || total > best.Total ||
                    total == best.Total && string.CompareOrdinal(plan.Id + addOn.Id, best.BasePlan.Id + best.AddOn.Id) < 0)
                {
                    best = new OfferRecommendationResponse(plan, addOn, total, request.Budget - total);
                }
            }
        }

        return best;
    }
}
