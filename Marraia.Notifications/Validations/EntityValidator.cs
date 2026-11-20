using FluentValidation.Results;
using Marraia.Notifications.Interfaces;

namespace Marraia.Notifications.Validations
{
    /// <summary>
    /// Base opcional para serviços que validam entidades e notificam os erros.
    /// </summary>
    public abstract class EntityValidator
    {
        protected EntityValidator(ISmartNotification smartNotification)
        {
            SmartNotification = smartNotification;
        }

        protected ISmartNotification SmartNotification { get; }

        public void NotifyValidationErrors(FieldValidation fieldValidation)
            => SmartNotification.Validation(fieldValidation);

        public void NotifyValidationErrors(ValidationResult validationResult)
            => SmartNotification.Validation(validationResult);
    }
}
