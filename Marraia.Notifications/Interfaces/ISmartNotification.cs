namespace Marraia.Notifications.Interfaces
{
    public interface ISmartNotification
    {
        bool IsValid();
        bool HasNotifications();
        void NewNotificationConflict(string message);
        void NewNotificationBadRequest(string message);
        void NewNotificationError(string message);
    }
}
