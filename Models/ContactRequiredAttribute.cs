using System.ComponentModel.DataAnnotations;

namespace AirBB.Models
{
    public class ContactRequiredAttribute : ValidationAttribute
    {
        public string OtherProperty { get; set; } = string.Empty;

        public ContactRequiredAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = value?.ToString() ?? string.Empty;
            
            // Get the other property value
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Unknown property: {OtherProperty}");
            }
            
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance)?.ToString() ?? string.Empty;
            
            // If both are empty, validation fails
            if (string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(otherValue))
            {
                return new ValidationResult(ErrorMessage ?? "Either Phone Number or Email must be provided.");
            }
            
            // At least one has content, validation passes
            return ValidationResult.Success;
        }
    }
}