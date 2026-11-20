using FluentValidation.Results;

namespace Marraia.Notifications.Validations
{
    /// <summary>
    /// Agrupa um ou mais resultados do FluentValidation.
    /// </summary>
    public class FieldValidation
    {
        public FieldValidation(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; private set; }
        public ICollection<ValidationResult> Validations { get; private set; } = new List<ValidationResult>();

        public void AddValidation(ValidationResult validationResult)
        {
            Validations.Add(validationResult);
        }

        public void AssignValid(bool isValid)
        {
            IsValid = isValid;
        }
    }
}
