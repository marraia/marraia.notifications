namespace Marraia.Notifications.Models
{
    /// <summary>
    /// Quando há notificações de tipos diferentes, a resposta usa o status mais grave:
    /// Error &gt; Unauthorized &gt; Forbidden &gt; NotFound &gt; Conflict &gt; BadRequest.
    /// </summary>
    public static class NotificationSeverity
    {
        private static readonly NotificationType[] Order =
        {
            NotificationType.Error,
            NotificationType.Unauthorized,
            NotificationType.Forbidden,
            NotificationType.NotFound,
            NotificationType.Conflict,
            NotificationType.BadRequest
        };

        public static NotificationType? MostSevere(IEnumerable<Notification> notifications)
        {
            var types = notifications.Select(n => n.Type).ToHashSet();
            foreach (var type in Order)
            {
                if (types.Contains(type))
                    return type;
            }

            return null;
        }
    }
}
