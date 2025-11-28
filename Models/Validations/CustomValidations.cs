using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AirBB.Models.Validations
{
    public class BuildYearValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object? value)
        {
            if (value is int year)
            {
                var currentYear = DateTime.Now.Year;
                var minimumYear = currentYear - 150;

                return year <= currentYear && year >= minimumYear;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            var currentYear = DateTime.Now.Year;
            var minimumYear = currentYear - 150;
            return $"Built year must be between {minimumYear} and {currentYear}";
        }

        // Client-side validation
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var currentYear = DateTime.Now.Year;
            var minimumYear = currentYear - 150;

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-buildyear", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
            MergeAttribute(context.Attributes, "data-val-buildyear-max", currentYear.ToString());
            MergeAttribute(context.Attributes, "data-val-buildyear-min", minimumYear.ToString());
        }

        private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }

    public class BathroomValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is decimal bathroom && bathroom > 0)
            {
                var fractionalPart = bathroom % 1;
                return fractionalPart == 0 || fractionalPart == 0.5m;
            }
            return true; // Let Required attribute handle null/empty
        }

        public override string FormatErrorMessage(string name)
        {
            return "Bathroom number must be a whole number or end with .5 (e.g., 1, 1.5, 2, 2.5)";
        }
    }

    public class AlphanumericWithSpacesAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[a-zA-Z0-9\s]+$");
            }
            return true; // Let Required attribute handle null/empty
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} can only contain letters, numbers, and spaces";
        }
    }
}