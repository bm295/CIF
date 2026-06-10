# AI-Native Implementation Plan

This document defines how AI coding agents should extend the CIF module in repeatable phases.

## 1) Architecture style

The repository is organized as **Clean Architecture** with dependency flow toward the domain:

- `Api` (HTTP contracts/endpoints and composition root)
- `Application` (use cases, commands/queries, DTO contracts, validation orchestration, ports)
- `Domain` (entities, value objects, business rules, domain-safe result primitives)
- `Infrastructure` (persistence, logging, integrations, and other adapters)

Dependency rules:

1. `Domain` must not reference any other project.
2. `Application` may reference `Domain` only.
3. `Infrastructure` implements ports declared by `Application`.
4. `Api` wires concrete infrastructure into application use cases.
5. Endpoints must call application use cases and never mutate domain objects directly.

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
- Never bypass domain rules from endpoints or infrastructure.
- Prefer explicit DTO mapping over implicit magic conversion.
- Return machine-readable validation errors (`code`, `field`, `message`).
- Avoid leaking sensitive values in exception paths.
- Keep adapter-specific concerns out of `Domain` and `Application`.

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

## 6) Suggested next PR sequence

- PR-1: relational persistence + optimistic concurrency
- PR-2: unit tests for domain rules and application use cases
- PR-3: integration tests for CRUD endpoints
- PR-4: privacy masking + audit trail hardening
