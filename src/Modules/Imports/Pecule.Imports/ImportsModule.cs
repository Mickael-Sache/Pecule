using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pecule.Imports
{
    public static class ImportsModule
    {
        public static IServiceCollection AddImportsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Pecule") ?? throw new InvalidOperationException("La chaîne de connexion « Pecule » est absente.");

            services.AddDbContext<ImportsDbContext>(options => options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "imports")));

            // Register services related to the Imports module here
            // Example: services.AddScoped<IImportService, ImportService>();
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
