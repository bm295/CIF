using CIF.Application.Assistant;
using CIF.Infrastructure.Assistant;
using CIF.Api.Endpoints;
using CIF.Infrastructure.Offers;
using CIF.Application.Common;
using CIF.Application.CustomerProfiles.UseCases;
using CIF.Infrastructure.Persistence;
using CIF.Application.CustomerProfiles.Offers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddSingleton<ICustomerProfileRepository, InMemoryCustomerProfileRepository>();
builder.Services.AddScoped<CustomerProfileService>();
builder.Services.AddSingleton<IOfferAcceptanceScorer>(_ =>
    new JsonOfferAcceptanceScorer(Path.Combine(AppContext.BaseDirectory, "Models", "offer-acceptance.json")));
builder.Services.AddSingleton<OfferRecommendationService>();

// Disabled by default: this MVP has no authentication or rate limiting.
if (builder.Configuration["LLM_ASSISTANT_ENABLED"] == "true")
{
    var key = builder.Configuration["LLM_API_KEY"];
    var model = builder.Configuration["LLM_MODEL"];
    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(model))
        throw new InvalidOperationException("LLM_API_KEY and LLM_MODEL are required when assistant is enabled.");

    builder.Services.AddHttpClient<ILanguageModel, OpenAiCompatibleLanguageModel>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["LLM_BASE_URL"] ?? "https://api.openai.com/v1/");
        client.Timeout = TimeSpan.FromSeconds(20);
    }).AddTypedClient<ILanguageModel>((http, _) => new OpenAiCompatibleLanguageModel(http, key, model));
    builder.Services.AddScoped<CifAssistantService>();
}

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Configuration["LLM_ASSISTANT_ENABLED"] == "true")
    app.MapCifAssistantEndpoints();

app.MapCustomerProfileEndpoints();
app.MapOfferRecommendationEndpoints();
app.Run();

