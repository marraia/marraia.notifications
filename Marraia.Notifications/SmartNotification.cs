using Marraia.Notifications.Handlers;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using Marraia.Notifications.Models.Enum;
using MediatR;
using System.Threading;

namespace Marraia.Notifications
{
    public class SmartNotification : ISmartNotification
    {
        private readonly DomainNotificationHandler _messageHandler;

        public SmartNotification(INotificationHandler<DomainNotification> notification)
        {
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

            _messageHandler.Handle(new DomainNotification(message), default(CancellationToken));
        }

        public void NewNotificationBadRequest(string message)
        {
            if (message == null)
                return;

            _messageHandler.Handle(new DomainNotification(message, DomainNotificationType.BadRequest), default(CancellationToken));
        }

    }
}
