using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pecule.Notifications
{
    public static class NotificationsModule
    {
        public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register services related to the Notifications module here
            // Example: services.AddScoped<INotificationService, NotificationService>();
            return services;
        }
    }
}