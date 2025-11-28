using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirBB.Models
{
    internal class ConfigureReservation : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> entity)
        {
            entity.HasData(
                new Reservation
                {
                    ReservationID = 1,
                    ReservationStartDate = new DateTime(2024, 1, 1),
                    ReservationEndDate = new DateTime(2024, 1, 5),
                    ResidenceID = 1
                }
            );
        }
    }
}
