using System.ComponentModel.DataAnnotations;

namespace AirBB.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }

        // ReservationStartDate
        public DateTime ReservationStartDate { get; set; }

        // ReservationEndDate
        public DateTime ReservationEndDate { get; set; }

        // ResidenceId (FK)
        public int ResidenceID { get; set; }

        // Residence navigation property
        public Residence Residence { get; set; } = null!;
    }
}