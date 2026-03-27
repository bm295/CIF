# Product Requirements — Customer Information (CIF / Profile)

## 1) Objective

Build a CRM-upgrade style Customer Information module that supports:

- Customer profile management
- Basic KYC capture (name, address, identity document)
- Strong input validation
- Privacy-aware data handling

## 2) In-scope

### Functional

1. Create customer profile
2. Read customer profile by id
3. Update customer profile
4. Soft delete/deactivate profile
5. List/search profiles with simple filters

### Data points (minimum)

- `customerId` (system generated)
- `fullName`
- `dateOfBirth` (optional in first release)
- `phoneNumber` (optional)
- `email` (optional)
- `address` (structured)
- `governmentIdType`
- `governmentIdNumber`
- `kycStatus` (NotSubmitted, Pending, Verified, Rejected)

### Non-functional

- API-first contracts
- Validation with clear error messages
- Basic privacy controls (masking/log hygiene)
- Auditable metadata (`createdAt`, `updatedAt`, `updatedBy`)

## 3) Out-of-scope (phase 1)

- Full AML/sanctions screening
- Complex risk scoring
- OCR/document upload pipeline
- Multi-country legal policy engine

## 4) Personas

- **Operations staff**: creates/updates customer profile records
- **Support staff**: searches and views customer profile details
- **Auditor/compliance reviewer**: verifies profile history and field-level changes

## 5) User stories

1. As operations staff, I can create a profile with mandatory fields and get immediate validation feedback.
2. As support staff, I can view profile details with sensitive values masked where required.
3. As operations staff, I can update profile attributes while preserving audit history.
4. As compliance reviewer, I can inspect KYC status changes over time.

## 6) Acceptance criteria (phase 1)

- CRUD endpoints are available and documented.
- Invalid payloads return standardized validation errors.
- Sensitive ID values are not fully exposed in logs or default list responses.
- Soft-deleted profiles are excluded from default list/search queries.
