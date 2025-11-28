using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using AirBB.Models.Validations;

namespace AirBB.Models
{
    public class Residence : IValidatableObject
    {
        public int ResidenceID { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must be less than 50 characters")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Picture Filename")]
        public string ResidencePicture { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a location")]
        public int LocationID { get; set; }
        
        [Required(ErrorMessage = "Owner is required")]
        [Display(Name = "Owner")]
        [Remote(action: "ValidateOwner", controller: "Manage", areaName: "Admin", ErrorMessage = "Please select an existing owner")]
        public int ClientID { get; set; }
        
        // Navigation Properties
        public Location? Location { get; set; }
        public Client? Client { get; set; }
        
        [Required(ErrorMessage = "Build year is required")]
        [Display(Name = "Built Year")]
        [BuildYearValidation]
        public int BuildYear { get; set; }
        
        [Required(ErrorMessage = "Guest number is required")]
        [Display(Name = "Accommodation (Guests)")]
        [Range(1, 50, ErrorMessage = "Guest number must be between 1 and 50")]
        public int GuestNumber { get; set; }
        
        [Required(ErrorMessage = "Bedroom number is required")]
        [Display(Name = "Bedrooms")]
        [Range(0, 20, ErrorMessage = "Bedroom number must be between 0 and 20")] // 0 is valid for studio apartments
        public int BedroomNumber { get; set; }
        
        [Required(ErrorMessage = "Bathroom number is required")]
        [Display(Name = "Bathrooms")]
        [Range(0.5, 20.0, ErrorMessage = "Bathroom number must be between 0.5 and 20")]
        public decimal BathroomNumber { get; set; }
        
        [Required(ErrorMessage = "Price per night is required")]
        [Display(Name = "Price Per Night ($)")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $10,000")]
        public decimal PricePerNight { get; set; }
        
        // Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Name validation - alphanumeric with spaces only
            if (!string.IsNullOrEmpty(Name) && !System.Text.RegularExpressions.Regex.IsMatch(Name, @"^[a-zA-Z0-9\s]+$"))
            {
                yield return new ValidationResult(
                    "Name can only contain letters, numbers, and spaces",
                    new[] { nameof(Name) });
            }
            
            // Bathroom validation - must be whole number or end with .5
            if (BathroomNumber > 0)
            {
                var fractionalPart = BathroomNumber % 1;
                if (fractionalPart != 0 && fractionalPart != 0.5m)
                {
                    yield return new ValidationResult(
                        "Bathroom number must be a whole number or end with .5 (e.g., 1, 1.5, 2, 2.5)",
                        new[] { nameof(BathroomNumber) });
                }
            }
        }
    }
}
