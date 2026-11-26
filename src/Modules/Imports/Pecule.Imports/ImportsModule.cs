using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Pecule.Accounts.Contracts;
using Pecule.SharedKernel;

namespace Pecule.Imports
{
    public static class ImportsModule
    {
        public static IServiceCollection AddImportsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Pecule") ?? throw new InvalidOperationException("La chaîne de connexion « Pecule » est absente.");

            services.AddDbContext<ImportsDbContext>(options => options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "imports")));

            services.AddScoped<IEventSubscriber<UserDeleted>, DeleteUserImports>();

            return services;
        }

        public static async Task MigrateImportsDatabaseAsync(this IServiceProvider services)
        {
            await using var scope = services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ImportsDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
