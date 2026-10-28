using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pecule.Insights
{
    public static class InsightsModule
    {
        public static IServiceCollection AddInsightsModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register services related to the Insights module here
            // Example: services.AddScoped<IInsightService, InsightService>();
            return services;
        }
    }
}
