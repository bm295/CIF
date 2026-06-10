# Customer Information Service (CIF / Profile)

This repository now uses a **Clean Architecture** layout for a Customer Information (CIF/Profile) service.
The old threading-demo console app has been replaced by layered projects that keep domain rules independent
from HTTP and persistence concerns.

## Architecture

```text
Api  ───────────────▶ Application ───────────────▶ Domain
 │                         ▲
 └──────────────▶ Infrastructure ┘
```

- `Domain/` — enterprise rules: customer profile aggregate, value objects, KYC transitions, and reusable result/error primitives.
- `Application/` — use cases and ports: DTO contracts, customer profile service, repository port, and clock abstraction.
- `Infrastructure/` — adapters: in-memory customer profile repository used as the first persistence adapter.
- `Api/` — delivery mechanism: ASP.NET Core minimal API endpoints under `/api/v1/customers`.
- `docs/` — product, domain, API, privacy, and AI-native implementation guidance.

## Implemented service capabilities

- Create a customer profile.
- Read a customer profile by id.
- Full-update a customer profile through the application use case.
- Soft-delete/deactivate a customer profile.
- Search active profiles by keyword and KYC status.
- Return machine-readable validation errors without leaking sensitive exception details.

## Boundary rules

- `Domain` has no dependencies on other projects.
- `Application` depends only on `Domain`.
- `Infrastructure` depends on `Application` and `Domain` to implement application ports.
- `Api` composes the application service and infrastructure adapter.
- Controllers/endpoints must not bypass domain aggregate methods or value object factories.

## Run locally

> The project targets `.NET 10` preview to match the existing repository configuration.

```bash
dotnet run --project Api/Api.csproj
```

Example create request:

```bash
curl -X POST http://localhost:5000/api/v1/customers \
  -H 'Content-Type: application/json' \
  -d '{
    "fullName": "Nguyen Van A",
    "dateOfBirth": "1992-02-03",
    "phoneNumber": "+84901234567",
    "email": "a@example.com",
    "address": {
      "line1": "123 Nguyen Trai",
      "districtOrCity": "Ho Chi Minh City",
      "countryCode": "VN"
    },
    "governmentId": {
      "type": "NationalId",
      "number": "079123456789",
      "issuingCountryCode": "VN"
    },
    "actor": "system"
  }'
```

## Next steps

- Replace the in-memory repository with relational persistence and optimistic concurrency.
- Add unit, integration, and contract tests.
- Add audit log persistence for profile changes.
- Harden privacy controls for masked search results and safe logging.
