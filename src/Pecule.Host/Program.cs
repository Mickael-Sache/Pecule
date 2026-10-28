using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore.Storage;

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
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

var app = builder.Build();

// Migrate the database on startup if configured to do so
if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await app.Services.MigrateAccountsDatabaseAsync();
}

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

app.MapOpenApi(); // OpenAPI (Swagger) endpoint
app.MapScalarApiReference(); // Scalar API reference endpoint
app.MapHealthChecks("/health"); // Health check endpoint

// Add an endpoint filter to the /api group to validate antiforgery tokens for non-GET requests
var api = app.MapGroup("/api").AddEndpointFilter(async (context, next) =>
{
    var request = context.HttpContext.Request;
    if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method) && !HttpMethods.IsOptions(request.Method))
    {
        var antiforgery = context.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
        if (!await antiforgery.IsRequestValidAsync(context.HttpContext))
        {
            return TypedResults.Problem("Antiforgery token is invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }

    return await next(context);
});

// Add an endpoint to generate and return an antiforgery token
api.MapGet("/antiforgery/token", (IAntiforgery antiforgery, HttpContext context) =>
{
    var tokens = antiforgery.GetAndStoreTokens(context);
    // GetAndStoreTokens always produces a RequestToken, so we can safely use the null-forgiving operator here.
    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
    {
        HttpOnly = false,
        SameSite = SameSiteMode.Strict,
        Secure = context.Request.IsHttps,
    });
    return TypedResults.NoContent();
});

api.MapGet("/status", () => new { Application = "Pécule", Version = "0.1.0" });
api.MapAccountEndpoints(); // Map endpoints related to the Accounts module

app.MapGet("/", () => "Pécule");

app.UseStaticFiles(); // Angular files will be served from wwwroot folder
app.MapFallbackToFile("index.html");

app.Run();