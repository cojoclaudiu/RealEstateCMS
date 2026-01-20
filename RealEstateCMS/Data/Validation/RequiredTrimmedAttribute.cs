using System.ComponentModel.DataAnnotations;

namespace RealEstateCMS.Data.Validation
{
    public class RequiredTrimmedAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            // Check for null OR whitespace
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                return new ValidationResult(
                    ErrorMessage ?? "Câmpul este obligatoriu.");
            }

            return ValidationResult.Success;
        }
    }
}