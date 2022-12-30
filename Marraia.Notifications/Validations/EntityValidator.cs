using Marraia.Notifications.Interfaces;

namespace Marraia.Notifications.Validations
{
    public abstract class EntityValidator
    {
        private readonly ISmartNotification _smartNotification;
        public EntityValidator(ISmartNotification smartNotification)
        {
            _smartNotification = smartNotification;
        }

        public void NotifyValidationErrors(FieldValidation fieldValidation)
        {
            foreach (var item in fieldValidation.Validations)
            {
                foreach (var validations in item.Errors)
                {
                    _smartNotification
                        .NewNotificationBadRequest(validations.ErrorMessage);
                }
            }
        }
    }
}
