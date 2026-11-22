# CIF LLM Assistant MVP

An opt-in, read-only educational assistant for general CIF/Profile and KYC questions.

## Configuration

Set environment variables `LLM_API_KEY` and `LLM_MODEL`; optionally set `LLM_BASE_URL` (defaults to `https://api.openai.com/v1/`). The API key must be stored in a secret manager, never committed.

The endpoint is only registered when both key and model are configured.

## Example

```bash
curl -X POST http://localhost:5000/api/v1/assistant/ask \
  -H 'Content-Type: application/json' \
  -d '{"question":"What is the purpose of KYC status in a customer profile?"}'
```

## Architecture

Api -> Application.CifAssistantService -> Application.ILanguageModel <- Infrastructure.OpenAiCompatibleLanguageModel.

The assistant does not query the customer repository or mutate profiles. Do not submit PII or confidential data in questions. In production, add authentication, authorization, rate limiting, input PII detection/redaction, output moderation, audit metadata without prompt content, and provider data-retention review before enabling for external users.

## Validation

Run `dotnet build CIF.sln`. Test a normal conceptual question, empty/oversized question (400), and upstream outage (503). The model response is not authoritative for compliance decisions.
