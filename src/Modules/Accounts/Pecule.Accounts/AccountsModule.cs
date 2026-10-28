using System.Security.Claims;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Pecule.Accounts.Contracts;

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

        // Register validators from the assembly containing CreateAccountRequestValidator
        services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>(includeInternalTypes: true);

        // Register IHttpContextAccessor and CurrentUser for accessing the current user's information
        services.AddHttpContextAccessor();
        // Register CurrentUser as a scoped service to access the current user's ID
        services.AddScoped<CurrentUser>();

        return services;
    }

    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder api)
    {
        // -- /api/auth endpoints
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

        // -- /api/accounts endpoints
        var accounts = api.MapGroup("/accounts").RequireAuthorization();

        accounts.MapGet("/", async (AccountsDbContext db) =>
            await db.Accounts
                .OrderBy(a => a.Name)
                .Select(a => new AccountDto(a.Id, a.Name, a.Type, a.InitialBalance, a.Currency))
                .ToListAsync());

        accounts.MapPost("/", async Task<Results<Created<AccountDto>, ValidationProblem, UnauthorizedHttpResult>> (
            CreateAccountRequest request,
            IValidator<CreateAccountRequest> validator,
            CurrentUser currentUser,
            AccountsDbContext db) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return TypedResults.ValidationProblem(validation.ToDictionary());
            }

            if (currentUser.Id is not { } userId)
            {
                return TypedResults.Unauthorized();
            }

            var account = new Account
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Name = request.Name.Trim(),
                Type = request.Type,
                InitialBalance = request.InitialBalance,
            };
            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            var dto = new AccountDto(account.Id, account.Name, account.Type, account.InitialBalance, account.Currency);
            return TypedResults.Created($"/api/accounts/{account.Id}", dto);
        });

        return api;
    }

    public static async Task MigrateAccountsDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
