# Customer Information Service (CIF / Profile)

This repository is being rewritten from a threading demo into a **Customer Information (CIF/Profile)** service blueprint.

Current focus: **documentation-first, AI-native architecture** so implementation agents can build consistently and safely.

## Target scope

A CRM-upgrade style module for storing and managing customer profile data:

- Customer information (CIF)
- Basic KYC fields (name, address, ID document)
- CRUD operations
- Validation rules
- Privacy-aware data handling (without complex compliance engines)

## Why docs-first

Before coding, we define:

1. Domain boundaries and entities
2. API contracts and error model
3. Validation and privacy rules
4. AI implementation workflow (tasks, prompts, guardrails)

This lets AI agents generate code with less ambiguity and more consistency.

## Documentation map

- `docs/00-product-requirements.md` — product scope and requirements baseline
- `docs/01-domain-model.md` — domain entities, value objects, and lifecycle
- `docs/02-ai-native-implementation-plan.md` — architecture and AI execution plan
- `docs/03-api-spec.md` — REST contract for CRUD + validation behavior
- `docs/04-privacy-and-kyc.md` — privacy controls and KYC data policy

## Proposed implementation stack (next step)

- .NET API for service layer
- Relational DB for CIF profile persistence
- Structured validation pipeline
- Audit-friendly change tracking

## Notes

Legacy demo files are still present during transition. The first milestone is to lock down architecture/docs, then implement in iterative PRs.
