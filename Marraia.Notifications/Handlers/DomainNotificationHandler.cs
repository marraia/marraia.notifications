using Marraia.Notifications.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marraia.Notifications.Handlers
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        private List<DomainNotification> _notifications;

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }
        public virtual Task Handle(DomainNotification notification, CancellationToken cancellationToken)
        {
            _notifications.Add(notification);
            return Task.CompletedTask;
        }

        public virtual List<DomainNotification> GetNotifications()
        {
            return _notifications
                        .Where(not => not.GetType() == typeof(DomainNotification))
                        .ToList();
        }

        public virtual bool HasNotifications()
        {
            return GetNotifications()
                    .Any();
        }

        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
            GC.SuppressFinalize(this);
        }
    }
}
