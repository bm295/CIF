namespace CIF.Application.Assistant;

public interface ILanguageModel
{
    Task<string> GenerateAsync(string systemPrompt, string question, CancellationToken cancellationToken);
}
