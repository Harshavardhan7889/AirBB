using System.ComponentModel.DataAnnotations;

namespace AirBB.Models
{
    public class Location : IValidatableObject
    {
        public int LocationID { get; set; }
        
        [Required(ErrorMessage = "Location name is required")]
        [StringLength(100, ErrorMessage = "Location name must be less than 100 characters")]
        [Display(Name = "Location Name")]
        public string Name { get; set; } = string.Empty;
        
        // Custom validation for duplicate location names
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // This will be handled by remote validation and controller logic
            yield break;
        }
    }
}