using CIF.Domain.Common;

namespace CIF.Domain.CustomerProfiles;

public sealed class CustomerProfile
{
    private CustomerProfile(
        Guid customerId,
        string fullName,
        DateOnly? dateOfBirth,
        string? phoneNumber,
        string? email,
        Address address,
        GovernmentId governmentId,
        KycStatus kycStatus,
        bool isActive,
        DateTimeOffset createdAtUtc,
        string createdBy,
        DateTimeOffset updatedAtUtc,
        string updatedBy)
    {
        CustomerId = customerId;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        GovernmentId = governmentId;
        KycStatus = kycStatus;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        CreatedBy = createdBy;
        UpdatedAtUtc = updatedAtUtc;
        UpdatedBy = updatedBy;
    }

    public Guid CustomerId { get; }
    public string FullName { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public Address Address { get; private set; }
    public GovernmentId GovernmentId { get; private set; }
    public KycStatus KycStatus { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }
    public string CreatedBy { get; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public string UpdatedBy { get; private set; }

    public static Result<CustomerProfile> Create(
        string? fullName,
        DateOnly? dateOfBirth,
        string? phoneNumber,
        string? email,
        Address address,
        GovernmentId governmentId,
        string actor,
        DateTimeOffset nowUtc)
    {
        var validation = ValidateProfile(fullName, phoneNumber, email, actor);
        if (validation.IsFailure)
        {
            return Result<CustomerProfile>.Failure([.. validation.Errors]);
        }

        var profile = new CustomerProfile(
            Guid.NewGuid(),
            fullName!.Trim(),
            dateOfBirth,
            NormalizeOptional(phoneNumber),
            NormalizeOptional(email)?.ToLowerInvariant(),
            address,
            governmentId,
            KycStatus.NotSubmitted,
            true,
            nowUtc,
            actor.Trim(),
            nowUtc,
            actor.Trim());

        return Result<CustomerProfile>.Success(profile);
    }

    public Result Update(
        string? fullName,
        DateOnly? dateOfBirth,
        string? phoneNumber,
        string? email,
        Address address,
        GovernmentId governmentId,
        string actor,
        DateTimeOffset nowUtc)
    {
        if (!IsActive)
        {
            return Result.Failure(new Error("InactiveProfile", "customerId", "Inactive profiles cannot be updated."));
        }

        var validation = ValidateProfile(fullName, phoneNumber, email, actor);
        if (validation.IsFailure)
        {
            return validation;
        }

        FullName = fullName!.Trim();
        DateOfBirth = dateOfBirth;
        PhoneNumber = NormalizeOptional(phoneNumber);
        Email = NormalizeOptional(email)?.ToLowerInvariant();
        Address = address;
        GovernmentId = governmentId;
        Touch(actor, nowUtc);

        return Result.Success();
    }

    public Result ChangeKycStatus(KycStatus nextStatus, string actor, DateTimeOffset nowUtc)
    {
        var allowed = KycStatus switch
        {
            KycStatus.NotSubmitted => nextStatus is KycStatus.Pending,
            KycStatus.Pending => nextStatus is KycStatus.Verified or KycStatus.Rejected,
            KycStatus.Rejected => nextStatus is KycStatus.Pending,
            KycStatus.Verified => false,
            _ => false
        };

        if (!allowed)
        {
            return Result.Failure(new Error(
                "InvalidKycTransition",
                "kycStatus",
                $"Cannot transition KYC status from {KycStatus} to {nextStatus}."));
        }

        KycStatus = nextStatus;
        Touch(actor, nowUtc);
        return Result.Success();
    }

    public void Deactivate(string actor, DateTimeOffset nowUtc)
    {
        IsActive = false;
        Touch(actor, nowUtc);
    }

    private void Touch(string actor, DateTimeOffset nowUtc)
    {
        UpdatedAtUtc = nowUtc;
        UpdatedBy = actor.Trim();
    }

    private static Result ValidateProfile(string? fullName, string? phoneNumber, string? email, string actor)
    {
        List<Error> errors = [];

        if (string.IsNullOrWhiteSpace(fullName))
        {
            errors.Add(new Error("Required", "fullName", "Full name is required."));
        }
        else if (fullName.Trim().Length > 200)
        {
            errors.Add(new Error("MaxLength", "fullName", "Full name must be 200 characters or fewer."));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber) && string.IsNullOrWhiteSpace(email))
        {
            errors.Add(new Error("ContactRequired", "contact", "At least one contact method is required."));
        }

        if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@', StringComparison.Ordinal))
        {
            errors.Add(new Error("InvalidEmail", "email", "Email address is invalid."));
        }

        if (string.IsNullOrWhiteSpace(actor))
        {
            errors.Add(new Error("Required", "actor", "Actor is required for audit metadata."));
        }

        return errors.Count == 0 ? Result.Success() : Result.Failure([.. errors]);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
