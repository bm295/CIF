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

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapCustomerProfileEndpoints();
app.MapOfferRecommendationEndpoints();
app.Run();

