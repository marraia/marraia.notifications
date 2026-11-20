using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Marraia.Notifications.Configurations
{
    public static class SmartNotificationExtensions
    {
        /// <summary>
        /// Registra o <see cref="ISmartNotification"/> com escopo por requisição.
        /// </summary>
        public static IServiceCollection AddSmartNotification(this IServiceCollection services)
        {
            services.AddScoped<ISmartNotification, SmartNotification>();
            return services;
        }

        /// <summary>
        /// Faz os erros de binding/validação automática do [ApiController] (JSON inválido,
        /// campo com tipo errado) saírem no mesmo formato das notificações, com status 400.
        /// </summary>
        public static IMvcBuilder AddSmartNotificationModelStateResponse(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var entries = context.ModelState
                        .Where(entry => entry.Value is { Errors.Count: > 0 })
                        .ToList();

                    // Com JSON inválido o ASP.NET também acusa "The {parâmetro} field is required.";
                    // se já existe um erro mais específico, esse aviso genérico é descartado.
                    var parameterNames = context.ActionDescriptor.Parameters
                        .Select(p => p.Name)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    if (entries.Any(e => !parameterNames.Contains(e.Key)))
                        entries = entries.Where(e => !parameterNames.Contains(e.Key)).ToList();

                    var notifications = entries
                        .SelectMany(entry => entry.Value!.Errors.Select(error => new Notification(
                            string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Valor inválido" : error.ErrorMessage,
                            NotificationType.BadRequest,
                            NormalizeField(entry.Key))));

                    return new BadRequestObjectResult(ErrorResponse.From(notifications));
                };
            });

            return builder;
        }

        // "$.name" / "input.Name" / "Name" -> "name"
        private static string? NormalizeField(string key)
        {
            var field = key.StartsWith("$.", StringComparison.Ordinal) ? key[2..] : key;
            if (string.IsNullOrWhiteSpace(field) || field == "$")
                return null;

            return string.Join('.', field.Split('.').Select(part =>
                part.Length == 0 ? part : char.ToLowerInvariant(part[0]) + part[1..]));
        }
    }
}
