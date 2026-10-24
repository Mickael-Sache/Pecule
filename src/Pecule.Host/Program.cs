using Pecule.Accounts;
using Pecule.Imports;
using Pecule.Insights;
using Pecule.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAccountsModule(builder.Configuration)
    .AddImportsModule(builder.Configuration)
    .AddInsightsModule(builder.Configuration)
    .AddNotificationsModule(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Pécule");

app.Run();
