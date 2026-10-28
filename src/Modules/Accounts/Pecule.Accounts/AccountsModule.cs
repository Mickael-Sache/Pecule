using System.Security.Claims;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pecule.Accounts;

public static partial class AccountsModule
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register services related to the Accounts module here
        // Example: services.AddScoped<IAccountService, AccountService>();

        // Set the schema to "accounts" for the migration history table (one per module)
        services.AddDbContext<AccountsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Pecule"),
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "accounts")));

        // Password and user settings for Identity
        services.AddIdentityApiEndpoints<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 12;
            options.Password.RequiredUniqueChars = 1;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<AccountsDbContext>();

        // Configure cookie settings for authentication
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "pecule.auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
        });

        services.AddAuthorization();

        return services;
    }

    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder api)
    {
        var auth = api.MapGroup("/auth");
        auth.MapIdentityApi<User>();

        auth.MapPost("/logout", async (SignInManager<User> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.NoContent();
        }).RequireAuthorization();

        auth.MapDelete("/account", async Task<Results<NoContent, UnauthorizedHttpResult, ProblemHttpResult>> (ClaimsPrincipal principal, UserManager<User> userManager, SignInManager<User> signInManager) =>
        {
            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return TypedResults.Problem("Failed to delete the account.");
            }

            await signInManager.SignOutAsync();
            return TypedResults.NoContent();
        }).RequireAuthorization();

        return api;
    }

    public static async Task MigrateAccountsDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
