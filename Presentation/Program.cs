using Data.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Services;


var builder = FunctionsApplication.CreateBuilder(args);

var sqlConnection = builder.Configuration["SqlConnection"];

builder.ConfigureFunctionsWebApplication();
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddContexts(sqlConnection)
    .AddScoped<IProfileService, ProfileService>();

builder.Build().Run();