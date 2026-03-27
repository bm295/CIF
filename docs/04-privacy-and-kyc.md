# Privacy and KYC Handling Guidelines

## 1) Data classification

- **Public/internal operational**: customer id, kyc status, timestamps
- **Sensitive personal data**: name, address, phone, email
- **Highly sensitive identifiers**: government ID number

## 2) Minimum privacy controls (phase 1)

1. Mask government ID in non-detail endpoints.
   - Example: `079123******`
2. Do not log raw government ID values.
3. Restrict profile detail endpoint to authorized roles.
4. Record who changed KYC status and when.

## 3) KYC lifecycle (basic)

- Initial profile creation sets status `NotSubmitted` or `Pending` (configurable).
- Manual verification operation sets `Verified` or `Rejected`.
- Rejected profile can be re-submitted to `Pending`.

## 4) Audit expectations

Capture:

- actor/user id
- action type (`CREATE`, `UPDATE`, `KYC_STATUS_CHANGE`, `DEACTIVATE`)
- changed fields summary
- timestamp (UTC)

## 5) Security baseline

- Enforce TLS in transit.
- Encrypt database storage and backups.
- Apply least-privilege DB credentials.
- Add rate limiting for profile creation/update endpoints.
