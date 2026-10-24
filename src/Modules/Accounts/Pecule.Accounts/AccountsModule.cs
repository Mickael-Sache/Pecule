using System;
using System.Collections.Generic;
using System.Text;

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

        return services;
    }
}