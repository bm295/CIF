using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CIF.Application.Assistant;

namespace CIF.Infrastructure.Assistant;

public sealed class OpenAiCompatibleLanguageModel(HttpClient http, string apiKey, string model) : ILanguageModel
{
    public async Task<string> GenerateAsync(string systemPrompt, string question, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(new
        {
            model,
            temperature = 0.2,
            max_tokens = 400,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = question }
            }
        });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        using var payload = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var choices = payload.RootElement.GetProperty("choices");
        if (choices.GetArrayLength() == 0)
            throw new InvalidOperationException("Language model returned no choices.");
        return choices[0].GetProperty("message").GetProperty("content").GetString()
            ?? throw new InvalidOperationException("Language model returned no content.");
    }
}
