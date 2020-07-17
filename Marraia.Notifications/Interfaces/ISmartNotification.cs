namespace Marraia.Notifications.Interfaces
{
    public interface ISmartNotification
    {
        bool IsValid();
        void NewNotificationConflict(string message);
        void NewNotificationBadRequest(string message);
    }
}
