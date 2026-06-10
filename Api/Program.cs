using CIF.Api.Endpoints;
using CIF.Application.Common;
using CIF.Application.CustomerProfiles.UseCases;
using CIF.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddSingleton<ICustomerProfileRepository, InMemoryCustomerProfileRepository>();
builder.Services.AddScoped<CustomerProfileService>();

var app = builder.Build();


app.MapCustomerProfileEndpoints();
app.Run();
