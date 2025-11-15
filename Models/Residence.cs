using System.ComponentModel.DataAnnotations;
namespace AirBB.Models
{
    public class Residence
    {
        public int ResidenceID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ResidencePicture { get; set; } = string.Empty;
        public Location Location { get; set; } = null!;
        public Client Client { get; set; } = null!;
        public int BuildYear { get; set; }
        public int GuestNumber { get; set; }
        public int BedroomNumber { get; set; }
        public int BathroomNumber { get; set; }
        public decimal PricePerNight { get; set; }

    }
}
