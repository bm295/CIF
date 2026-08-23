using CIF.Application.Common;
using CIF.Application.CustomerProfiles.Contracts;
using CIF.Domain.Common;
using CIF.Domain.CustomerProfiles;

namespace CIF.Application.CustomerProfiles.UseCases;

public sealed class CustomerProfileService(
    ICustomerProfileRepository repository,
    ISystemClock clock)
{
    public async Task<Result<CustomerProfileResponse>> CreateAsync(
        UpsertCustomerProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var nowUtc = clock.UtcNow;
        var materialized = MaterializeValueObjects(request, nowUtc);
        if (materialized.IsFailure)
        {
            return Result<CustomerProfileResponse>.Failure([.. materialized.Errors]);
        }

        var materializedValue = materialized.Value!;
        var address = materializedValue.Address;
        var governmentId = materializedValue.GovernmentId;
        var profile = CustomerProfile.Create(
            request.FullName,
            request.DateOfBirth,
            request.PhoneNumber,
            request.Email,
            address,
            governmentId,
            request.Actor,
            nowUtc);

        if (profile.IsFailure)
        {
            return Result<CustomerProfileResponse>.Failure([.. profile.Errors]);
        }

        await repository.AddAsync(profile.Value!, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result<CustomerProfileResponse>.Success(profile.Value!.ToResponse());
    }

    public async Task<CustomerProfileResponse?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByIdAsync(customerId, cancellationToken);
        return profile?.ToResponse();
    }

    public async Task<Result<CustomerProfileResponse>> UpdateAsync(
        Guid customerId,
        UpsertCustomerProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByIdAsync(customerId, cancellationToken);
        if (profile is null)
        {
            return Result<CustomerProfileResponse>.Failure(new Error("NotFound", "customerId", "Customer profile was not found."));
        }

        var nowUtc = clock.UtcNow;
        var materialized = MaterializeValueObjects(request, nowUtc);
        if (materialized.IsFailure)
        {
            return Result<CustomerProfileResponse>.Failure([.. materialized.Errors]);
        }

        var materializedValue = materialized.Value!;
        var address = materializedValue.Address;
        var governmentId = materializedValue.GovernmentId;
        var update = profile.Update(
            request.FullName,
            request.DateOfBirth,
            request.PhoneNumber,
            request.Email,
            address,
            governmentId,
            request.Actor,
            nowUtc);

        if (update.IsFailure)
        {
            return Result<CustomerProfileResponse>.Failure([.. update.Errors]);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result<CustomerProfileResponse>.Success(profile.ToResponse());
    }

    public async Task<Result> DeactivateAsync(Guid customerId, string actor, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByIdAsync(customerId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(new Error("NotFound", "customerId", "Customer profile was not found."));
        }

        profile.Deactivate(actor, clock.UtcNow);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<PagedResult<CustomerProfileResponse>> SearchAsync(
        CustomerSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var profiles = await repository.SearchAsync(
            new CustomerProfileSearchCriteria(query.Keyword, query.KycStatus, page, pageSize, query.IncludeInactive),
            cancellationToken);

        return new PagedResult<CustomerProfileResponse>(
            [.. profiles.Select(profile => profile.ToResponse())],
            page,
            pageSize,
            profiles.Count);
    }

    private static Result<(Address Address, GovernmentId GovernmentId)> MaterializeValueObjects(
        UpsertCustomerProfileRequest request,
        DateTimeOffset nowUtc)
    {
        List<Error> errors = [];

        Result<Address>? address = null;
        if (request.Address is null)
        {
            errors.Add(new Error("Required", "address", "Address is required."));
        }
        else
        {
            address = Address.Create(
                request.Address.Line1,
                request.Address.Line2,
                request.Address.WardOrLocality,
                request.Address.DistrictOrCity,
                request.Address.StateOrProvince,
                request.Address.PostalCode,
                request.Address.CountryCode);

            if (address.IsFailure)
            {
                errors.AddRange(address.Errors);
            }
        }

        Result<GovernmentId>? governmentId = null;
        if (request.GovernmentId is null)
        {
            errors.Add(new Error("Required", "governmentId", "Government ID is required."));
        }
        else
        {
            governmentId = GovernmentId.Create(
                request.GovernmentId.Type,
                request.GovernmentId.Number,
                request.GovernmentId.IssuingCountryCode,
                request.GovernmentId.ExpiryDate,
                DateOnly.FromDateTime(nowUtc.UtcDateTime));

            if (governmentId.IsFailure)
            {
                errors.AddRange(governmentId.Errors);
            }
        }

        return errors.Count > 0
            ? Result<(Address Address, GovernmentId GovernmentId)>.Failure([.. errors])
            : Result<(Address Address, GovernmentId GovernmentId)>.Success((address!.Value!, governmentId!.Value!));
    }
}
