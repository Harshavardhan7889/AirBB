using System.ComponentModel.DataAnnotations;

namespace AirBB.Models
{
    public class ContactRequiredAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return true; // Individual field validation passes, cross-field validation in IValidatableObject
        }
    }
}