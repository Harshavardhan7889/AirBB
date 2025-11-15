using Microsoft.EntityFrameworkCore;

namespace AirBB.Models
{
    public class ResidenceContext : DbContext
    {
        public ResidenceContext(DbContextOptions<ResidenceContext> options)
            : base(options)
        { }

        public DbSet<Residence> Residences { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Location>().HasData(
                new Location { LocationID = 1, Name = "Denver" },
                new Location { LocationID = 2, Name = "Dallas" },
                new Location { LocationID = 3, Name = "Detroit" },
                new Location { LocationID = 4, Name = "Orlando" },
                new Location { LocationID = 5, Name = "Atlanta" },
                new Location { LocationID = 6, Name = "Chicago" },
                new Location { LocationID = 7, Name = "New York" }
            );

            modelBuilder.Entity<Client>().HasData(
                new Client 
                { 
                    ClientID = 1,
                    Name = "John Doe",
                    PhoneNumber = "555-0123",
                    Email = "john@example.com",
                    SSN = "123-45-6789",
                    UserType = "Owner",
                    DOB = new DateTime(1990, 1, 1)
                }
            );

            modelBuilder.Entity<Residence>().HasData(
                new 
                {
                    ResidenceID = 1,
                    Name = "Chicago Loop Apartment",
                    ResidencePicture = "chicago.jpg",
                    LocationID = 6,
                    ClientID = 1,
                    BuildYear = 2015,
                    GuestNumber = 4,
                    BedroomNumber = 2,
                    BathroomNumber = 1,
                    PricePerNight = 150.00m
                }
            );

            modelBuilder.Entity<Reservation>().HasData(
                new
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