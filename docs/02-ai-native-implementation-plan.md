# AI-Native Implementation Plan

This document defines how AI coding agents should implement the CIF module in repeatable phases.

## 1) Architecture style

Use **modular clean architecture** with clear boundaries:

- `Api` (HTTP contracts/controllers)
- `Application` (use-cases, commands/queries, validation)
- `Domain` (entities, value objects, business rules)
- `Infrastructure` (persistence, logging, integrations)

## 2) Implementation order (strict)

1. **Contracts first**
   - request/response DTOs
   - error response schema
2. **Domain model**
   - aggregate and value objects
   - status transition policies
3. **Validation layer**
   - field validators and business validators
4. **Persistence layer**
   - schema migrations
   - repository/query adapters
5. **Endpoints**
   - CRUD + list/search
6. **Privacy controls**
   - masking policy
   - safe logging guards
7. **Tests**
   - unit + integration + contract tests

## 3) Coding standards for AI agents

- Keep handlers/use-cases single-responsibility.
- Never bypass domain rules from controllers.
- Prefer explicit DTO mapping over implicit magic conversion.
- Return machine-readable validation errors (`code`, `field`, `message`).
- Avoid leaking sensitive values in exception paths.

## 4) Definition of done (per use-case)

Each endpoint is done only when:

- happy-path + invalid-path tests exist
- API docs are updated
- audit metadata is persisted correctly
- logging excludes/masks sensitive fields

## 5) AI task decomposition template

For each feature, break work into:

1. Contract task
2. Domain task
3. Validation task
4. Data access task
5. Endpoint wiring task
6. Test task
7. Documentation task

## 6) Suggested initial PR sequence

- PR-1: domain + DTO contracts + validation
- PR-2: persistence + migrations
- PR-3: CRUD endpoints + integration tests
- PR-4: privacy masking + audit trail hardening
