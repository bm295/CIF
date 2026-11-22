using CIF.Application.Assistant;

namespace CIF.Api.Endpoints;

public static class CifAssistantEndpoints
{
    public sealed record AssistantRequest(string? Question);
    public sealed record AssistantResponse(string Answer);

    public static IEndpointRouteBuilder MapCifAssistantEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/assistant/ask", async (
            AssistantRequest request, CifAssistantService assistant, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Question) || request.Question.Length > 1000)
                return Results.BadRequest(new { error = "Question must contain 1 to 1000 characters." });

            try
            {
                var answer = await assistant.AskAsync(request.Question, cancellationToken);
                return Results.Ok(new AssistantResponse(answer));
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return Results.Problem("Language model timed out.", statusCode: 504);
            }
            catch (HttpRequestException)
            {
                return Results.Problem("Language model is unavailable.", statusCode: 503);
            }
            catch (InvalidOperationException)
            {
                return Results.Problem("Language model response was invalid.", statusCode: 502);
            }
        });
        return app;
    }
}
