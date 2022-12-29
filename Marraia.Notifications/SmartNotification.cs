using Marraia.Notifications.Handlers;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using Marraia.Notifications.Models.Enum;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Marraia.Notifications
{
    public class SmartNotification : ISmartNotification
    {
        private ILogger<SmartNotification> _logger;
        private readonly DomainNotificationHandler _messageHandler;

        public SmartNotification(ILogger<SmartNotification> logger,
                                    INotificationHandler<DomainNotification> notification)
        {
            _logger = logger;
            _messageHandler = (DomainNotificationHandler)notification;
        }

        public bool IsValid()
        {
            return !_messageHandler
                        .HasNotifications();
        }

        public bool HasNotifications()
        {
            return _messageHandler
                        .HasNotifications();
        }

        public void NewNotificationConflict(string message)
        {
            if (message == null)
                return;

            _logger.LogWarning(message);
            _messageHandler.Handle(new DomainNotification(message), default(CancellationToken));
        }

        public void NewNotificationBadRequest(string message)
        {
            if (message == null)
                return;

            _logger.LogWarning(message);
            _messageHandler.Handle(new DomainNotification(message, DomainNotificationType.BadRequest), default(CancellationToken));
        }

        public void NewNotificationError(string message)
        {
            if (message == null)
                return;

            _logger.LogWarning(message);
            _messageHandler.Handle(new DomainNotification(message, DomainNotificationType.Error), default(CancellationToken));
        }

    }
}
