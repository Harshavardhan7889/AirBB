using System.ComponentModel.DataAnnotations;

namespace AirBB.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        // ReservationStartDate
        public DateTime ReservationStartDate { get; set; }

        // ReservationEndDate
        public DateTime ReservationEndDate { get; set; }

        // ResidenceId (FK)
        public string ResidenceID { get; set; } = string.Empty;

        // Residence navigation property
        public Residence Residence { get; set; } = null!;
    }
}