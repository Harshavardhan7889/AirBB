using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AirBB.Models.Validations
{
    public class ContactRequiredAttribute : ValidationAttribute, IClientModelValidator
    {
        public string OtherProperty { get; set; } = string.Empty;

        public ContactRequiredAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the value of the current property
            var currentValue = value?.ToString()?.Trim() ?? string.Empty;

            // Get the value of the other property
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Unknown property: {OtherProperty}");
            }

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance)?.ToString()?.Trim() ?? string.Empty;

            // If both are empty, validation fails
            if (string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(otherValue))
            {
                return new ValidationResult(ErrorMessage ?? "Either Phone Number or Email must be provided.");
            }

            // At least one has a value, validation passes
            return ValidationResult.Success;
        }

        // Add client-side validation support
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-contactrequired", ErrorMessage ?? "Either Phone Number or Email must be provided.");
            MergeAttribute(context.Attributes, "data-val-contactrequired-otherproperty", OtherProperty.ToLower());
        }

        private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}