using CIF.Application.CustomerProfiles.Offers;

namespace CIF.Api.Endpoints;

public static class OfferRecommendationEndpoints
{
    public static void MapOfferRecommendationEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/v1/offers/recommendation", RecommendAsync)
            .WithName("RecommendCustomerOffer")
            .WithTags("Offers");
    }

    private static IResult RecommendAsync(
        OfferRecommendationRequest request,
        OfferRecommendationService service)
    {
        if (request.Budget < 0 || request.BasePlans.Count == 0 || request.AddOns.Count == 0 ||
            request.BasePlans.Any(x => x.Price < 0) || request.AddOns.Any(x => x.Price < 0))
            return Results.BadRequest(new { error = "Budget must be non-negative and both option lists are required." });

        var recommendation = service.Recommend(request);
        return recommendation is null
            ? Results.NotFound(new { error = "No offer combination fits the budget." })
            : Results.Ok(recommendation);
    }
}

