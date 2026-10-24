using Pecule.Accounts;
using Pecule.Imports;
using Pecule.Insights;
using Pecule.Notifications;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAccountsModule(builder.Configuration)
    .AddImportsModule(builder.Configuration)
    .AddInsightsModule(builder.Configuration)
    .AddNotificationsModule(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("Pecule")!);
var app = builder.Build();

app.MapOpenApi(); // OpenAPI (Swagger) endpoint
app.MapScalarApiReference(); // Scalar API reference endpoint
app.MapHealthChecks("/health");

var api = app.MapGroup("/api");
api.MapGet("/status", () => new { Application = "Pécule", Version = "0.1.0" });

app.MapGet("/", () => "Pécule");

app.UseStaticFiles(); // Angular files will be served from wwwroot folder
app.MapFallbackToFile("index.html");

app.Run();