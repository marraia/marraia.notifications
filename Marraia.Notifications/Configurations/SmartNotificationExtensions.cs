using Marraia.Notifications.Handlers;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marraia.Notifications.Configurations
{
    public static class SmartNotificationExtensions
    {
        public static IServiceCollection AddSmartNotification(this IServiceCollection service)
        {
            service.AddScoped<ISmartNotification, SmartNotification>();
            service.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            return service;
        }
    }
}
