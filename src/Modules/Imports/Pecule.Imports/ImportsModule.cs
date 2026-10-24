using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pecule.Imports
{
    public static class ImportsModule
    {
        public static IServiceCollection AddImportsModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register services related to the Imports module here
            // Example: services.AddScoped<IImportService, ImportService>();
            return services;
        }
    }
}