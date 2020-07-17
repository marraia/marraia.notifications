using Marraia.Notifications.Models.Enum;
using MediatR;
using Newtonsoft.Json;

namespace Marraia.Notifications.Models
{
    public class DomainNotification : INotification
    {
        public string Value { get; }

        [JsonIgnore]
        public DomainNotificationType DomainNotificationType { get; }

        public DomainNotification(
            string value,
            DomainNotificationType type = DomainNotificationType.Conflict)
        {
            Value = value;
            DomainNotificationType = type;
        }
    }
}
