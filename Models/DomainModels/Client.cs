using System.ComponentModel.DataAnnotations;
using AirBB.Models.Validations;

namespace AirBB.Models
{
    public class Client : IValidatableObject
    {
        public int ClientID { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Phone Number")]
        [ContactRequired("Email", ErrorMessage = "Either Phone Number or Email must be provided.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        [ContactRequired("PhoneNumber", ErrorMessage = "Either Phone Number or Email must be provided.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "SSN is required")]
        [Display(Name = "SSN")]
        [RegularExpression(@"^\d{3}-?\d{2}-?\d{4}$", ErrorMessage = "SSN must be in format XXX-XX-XXXX")]
        public string SSN { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "User Type is required")]
        [Display(Name = "User Type")]
        public string UserType { get; set; } = string.Empty;
        
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; } = null;

        // Static property for UserType options
        public static Dictionary<string, string> UserTypeOptions => new Dictionary<string, string>
        {
            { "Owner", "Owner" },
            { "Admin", "Admin" },
            { "Client", "Client" }
        };

        // Simplified validation - only handle DOB and UserType
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate UserType against allowed options
            if (!string.IsNullOrEmpty(UserType) && !UserTypeOptions.ContainsKey(UserType))
            {
                yield return new ValidationResult(
                    "Please select a valid User Type.",
                    new[] { nameof(UserType) });
            }

            // DOB cannot be a future date
            if (DOB.HasValue && DOB.Value.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "Date of Birth cannot be a future date.",
                    new[] { nameof(DOB) });
            }

            // DOB cannot be too old (reasonable validation)
            if (DOB.HasValue && DOB.Value < DateTime.Today.AddYears(-150))
            {
                yield return new ValidationResult(
                    "Please enter a valid Date of Birth.",
                    new[] { nameof(DOB) });
            }
        }
    }
}