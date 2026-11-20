using System.Text.Json;
using FluentValidation.Results;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using Marraia.Notifications.Validations;
using Microsoft.Extensions.Logging;

namespace Marraia.Notifications
{
    public sealed class SmartNotification : ISmartNotification
    {
        private readonly List<Notification> _notifications = new();
        private readonly ILogger<SmartNotification> _logger;

        public SmartNotification(ILogger<SmartNotification> logger)
        {
            _logger = logger;
        }

        public bool IsValid => _notifications.Count == 0;
        public bool HasNotifications => _notifications.Count > 0;
        public IReadOnlyList<Notification> Notifications => _notifications.AsReadOnly();
        public NotificationType? MostSevereType => NotificationSeverity.MostSevere(_notifications);

        public void BadRequest(string message, string? field = null) => Add(message, NotificationType.BadRequest, field);
        public void Unauthorized(string message) => Add(message, NotificationType.Unauthorized);
        public void Forbidden(string message) => Add(message, NotificationType.Forbidden);
        public void NotFound(string message) => Add(message, NotificationType.NotFound);
        public void Conflict(string message) => Add(message, NotificationType.Conflict);
        public void Error(string message) => Add(message, NotificationType.Error);

        public void Validation(ValidationResult validationResult)
        {
            ArgumentNullException.ThrowIfNull(validationResult);

            foreach (var error in validationResult.Errors)
                BadRequest(error.ErrorMessage, ToFieldName(error.PropertyName));
        }

        public void Validation(FieldValidation fieldValidation)
        {
            ArgumentNullException.ThrowIfNull(fieldValidation);

            foreach (var result in fieldValidation.Validations)
                Validation(result);
        }

        public void Clear() => _notifications.Clear();

        private void Add(string message, NotificationType type, string? field = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var level = type == NotificationType.Error ? LogLevel.Error : LogLevel.Warning;
            _logger.Log(level, "Notificação {NotificationType}: {Message}", type, message);

            _notifications.Add(new Notification(message, type, field));
        }

        // "Admin.Email" -> "admin.email"; vazio -> null
        private static string? ToFieldName(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            return string.Join('.', propertyName.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
        }
    }
}
