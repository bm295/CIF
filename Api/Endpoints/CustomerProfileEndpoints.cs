using CIF.Api.Contracts;
using CIF.Application.CustomerProfiles.Contracts;
using CIF.Application.CustomerProfiles.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CIF.Api.Endpoints;

public static class CustomerProfileEndpoints
{
    public static RouteGroupBuilder MapCustomerProfileEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/customers")
            .WithTags("Customer Profiles");

        group.MapPost("/", CreateAsync)
            .WithName("CreateCustomerProfile");

        group.MapGet("/{customerId:guid}", GetByIdAsync)
            .WithName("GetCustomerProfileById");

        group.MapPut("/{customerId:guid}", UpdateAsync)
            .WithName("UpdateCustomerProfile");

        group.MapDelete("/{customerId:guid}", DeactivateAsync)
            .WithName("DeactivateCustomerProfile");

        group.MapGet("/", SearchAsync)
            .WithName("SearchCustomerProfiles");

        return group;
    }

    private static async Task<Results<Created<CustomerProfileResponse>, UnprocessableEntity<ValidationErrorResponse>>> CreateAsync(
        UpsertCustomerProfileRequest request,
        CustomerProfileService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return TypedResults.UnprocessableEntity(ValidationErrorResponse.FromErrors(result.Errors, httpContext.TraceIdentifier));
        }

        return TypedResults.Created($"/api/v1/customers/{result.Value!.CustomerId}", result.Value);
    }

    private static async Task<Results<Ok<CustomerProfileResponse>, NotFound>> GetByIdAsync(
        Guid customerId,
        CustomerProfileService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(customerId, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<CustomerProfileResponse>, NotFound, UnprocessableEntity<ValidationErrorResponse>>> UpdateAsync(
        Guid customerId,
        UpsertCustomerProfileRequest request,
        CustomerProfileService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(customerId, request, cancellationToken);
        if (result.IsFailure)
        {
            if (result.Errors.Any(error => error.Code == "NotFound"))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.UnprocessableEntity(ValidationErrorResponse.FromErrors(result.Errors, httpContext.TraceIdentifier));
        }

        return TypedResults.Ok(result.Value!);
    }

    private static async Task<Results<NoContent, NotFound, UnprocessableEntity<ValidationErrorResponse>>> DeactivateAsync(
        Guid customerId,
        string actor,
        CustomerProfileService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await service.DeactivateAsync(customerId, actor, cancellationToken);
        if (result.IsFailure)
        {
            if (result.Errors.Any(error => error.Code == "NotFound"))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.UnprocessableEntity(ValidationErrorResponse.FromErrors(result.Errors, httpContext.TraceIdentifier));
        }

        return TypedResults.NoContent();
    }

    private static async Task<Ok<PagedResult<CustomerProfileResponse>>> SearchAsync(
        CustomerProfileService service,
        string? keyword,
        CIF.Domain.CustomerProfiles.KycStatus? kycStatus,
        int page = 1,
        int pageSize = 20,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await service.SearchAsync(new CustomerSearchQuery(keyword, kycStatus, page, pageSize, includeInactive), cancellationToken);
        return TypedResults.Ok(result);
    }
}
