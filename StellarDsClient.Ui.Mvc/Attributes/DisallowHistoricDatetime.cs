using System.ComponentModel.DataAnnotations;

namespace StellarDsClient.Ui.Mvc.Attributes
{
    public class DisallowHistoricDatetime : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (!DateTime.TryParse(value.ToString(), out var inputValue)) return ValidationResult.Success;
     
            return inputValue < DateTime.UtcNow ? new ValidationResult($"The field {validationContext.DisplayName} must be greater than or equal to {DateTime.UtcNow}.") : ValidationResult.Success;
        }
    }
}