namespace CIF.Application.CustomerProfiles.Offers;

public sealed record OfferOption(string Id, string Name, decimal Price);

public sealed record OfferRecommendationRequest(
    decimal Budget,
    IReadOnlyCollection<OfferOption> BasePlans,
    IReadOnlyCollection<OfferOption> AddOns,
    bool UseAcceptanceModel = false);

public sealed record OfferRecommendationResponse(
    OfferOption BasePlan,
    OfferOption AddOn,
    decimal Total,
    decimal RemainingBudget,
    double? AcceptanceProbability = null);

public sealed class OfferRecommendationService
{
    private readonly IOfferAcceptanceScorer? scorer;

    public OfferRecommendationService(IOfferAcceptanceScorer? scorer = null) => this.scorer = scorer;

    public OfferRecommendationResponse? Recommend(OfferRecommendationRequest request)
    {
        if (request.Budget < 0)
            throw new ArgumentOutOfRangeException(nameof(request.Budget));

        if (request.BasePlans.Any(x => x.Price < 0) || request.AddOns.Any(x => x.Price < 0))
            throw new ArgumentException("Option prices must be non-negative.");
        if (request.UseAcceptanceModel && scorer is null)
            throw new InvalidOperationException("Offer acceptance model is not configured.");

        OfferRecommendationResponse? best = null;

        // Filter by budget before scoring; preserve price ranking unless the model is requested.
        foreach (var plan in request.BasePlans)
        {
            foreach (var addOn in request.AddOns)
            {
                var total = plan.Price + addOn.Price;
                if (total > request.Budget)
                    continue;

                double? probability = request.UseAcceptanceModel
                    ? scorer!.Score(new(request.Budget, plan.Price, addOn.Price)) : null;
                if (probability is double p && (!double.IsFinite(p) || p < 0 || p > 1))
                    throw new InvalidOperationException("Invalid model probability.");
                bool equalScore = probability == best?.AcceptanceProbability;
                if (best is null || probability > best.AcceptanceProbability ||
                    equalScore && (total > best.Total ||
                    total == best.Total && string.CompareOrdinal(plan.Id + addOn.Id, best.BasePlan.Id + best.AddOn.Id) < 0))
                {
                    best = new OfferRecommendationResponse(plan, addOn, total, request.Budget - total, probability);
                }
            }
        }

        return best;
    }
}

