using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;

namespace Pecule.IntegrationTests
{
    public sealed class PeculeApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:17").Build();

        public async Task InitializeAsync()
        {
            await _database.StartAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("ConnectionStrings:Pecule", _database.GetConnectionString());
            builder.UseSetting("Database:MigrateOnStartup", "true");
        }

        public new async Task DisposeAsync()
        {
            await _database.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
