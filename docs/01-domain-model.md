# Domain Model — CIF / Profile

## 1) Core aggregate

## CustomerProfile (Aggregate Root)

Fields:

- `CustomerId` (GUID/ULID)
- `FullName`
- `DateOfBirth?`
- `PhoneNumber?`
- `Email?`
- `Address` (Value Object)
- `GovernmentId` (Value Object)
- `KycStatus` (Enum)
- `IsActive` (bool)
- `CreatedAtUtc`
- `CreatedBy`
- `UpdatedAtUtc`
- `UpdatedBy`

## 2) Value objects

### Address

- `Line1` (required)
- `Line2` (optional)
- `WardOrLocality` (optional)
- `DistrictOrCity` (required)
- `StateOrProvince` (optional)
- `PostalCode` (optional but validated format if present)
- `CountryCode` (required, ISO-3166 alpha-2)

### GovernmentId

- `Type` (NationalId, Passport, DriverLicense, Other)
- `Number` (required, normalized)
- `IssuingCountryCode` (required)
- `ExpiryDate?`

## 3) Domain rules

- `FullName` is required and length-limited.
- At least one contact method (`phone` or `email`) should be present (configurable).
- `GovernmentId.Number` must pass per-type format validation.
- KYC status transitions must follow allowed flow:
  - `NotSubmitted -> Pending`
  - `Pending -> Verified | Rejected`
  - `Rejected -> Pending` (re-submit)

## 4) Domain events (optional, phase 2)

- `CustomerProfileCreated`
- `CustomerProfileUpdated`
- `CustomerKycStatusChanged`
- `CustomerProfileDeactivated`

## 5) Persistence mapping (logical)

- `customer_profiles` table
- `customer_profile_audit_logs` table

Use optimistic concurrency (`row_version` or equivalent) for update safety.
