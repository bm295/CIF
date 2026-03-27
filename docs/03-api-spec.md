# API Specification (Draft) — Customer Information

Base path: `/api/v1/customers`

## 1) Create customer

`POST /api/v1/customers`

### Request body

```json
{
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
  }
}
```

### Response

- `201 Created` + created object with `customerId`
- `400 Bad Request` for malformed JSON
- `422 Unprocessable Entity` for validation failures

## 2) Get customer by id

`GET /api/v1/customers/{customerId}`

- `200 OK`
- `404 Not Found`

## 3) Update customer

`PUT /api/v1/customers/{customerId}`

- full update semantics
- includes concurrency token (recommended)

Response:

- `200 OK`
- `404 Not Found`
- `409 Conflict` for concurrency conflict
- `422 Unprocessable Entity`

## 4) Deactivate customer (soft delete)

`DELETE /api/v1/customers/{customerId}`

- marks `isActive=false`
- preserves audit history

Response:

- `204 No Content`
- `404 Not Found`

## 5) List/search customers

`GET /api/v1/customers?keyword=&kycStatus=&page=1&pageSize=20`

Behavior:

- default excludes inactive profiles
- keyword searches by name/email/phone (masked-safe)
- supports pagination metadata

## 6) Validation error format

```json
{
  "error": "ValidationFailed",
  "details": [
    {
      "code": "Required",
      "field": "fullName",
      "message": "Full name is required."
    }
  ],
  "traceId": "00-..."
}
```
