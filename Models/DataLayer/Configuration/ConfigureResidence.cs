using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirBB.Models
{
    internal class ConfigureResidence : IEntityTypeConfiguration<Residence>
    {
        public void Configure(EntityTypeBuilder<Residence> entity)
        {
            entity.HasOne(r => r.Location)
                .WithMany()
                .HasForeignKey(r => r.LocationID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientID)
                .OnDelete(DeleteBehavior.Restrict);
            // seed initial data
            entity.HasData(
                new Residence
                {
                    ResidenceID = 1,
                    Name = "Chicago Loop Apartment",
                    ResidencePicture = "chicago.jpg",
                    LocationID = 6,
                    ClientID = 1,
                    BuildYear = 2015,
                    GuestNumber = 4,
                    BedroomNumber = 2,
                    BathroomNumber = 1.0m,
                    PricePerNight = 150.00m
                }
            );
        }
    }

}