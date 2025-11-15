using System.ComponentModel.DataAnnotations;

namespace AirBB.Validations
{
    public class BuildYearValidationAttribute : ValidationAttribute
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