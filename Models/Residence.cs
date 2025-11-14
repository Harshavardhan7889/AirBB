using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirBB.Models
{
    public class Residence
    {
        [Key]
        public string ResidenceID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ResidencePicture { get; set; } = string.Empty;
        public Client Client { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public int GuestNumber { get; set; }
        public int BedroomNumber { get; set; }
        public int BathroomNumber { get; set; }
        public decimal PricePerNight { get; set; }
        
    }
}
