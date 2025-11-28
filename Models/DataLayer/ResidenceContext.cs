using Microsoft.EntityFrameworkCore;

namespace AirBB.Models.DataLayer
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
            //base.OnModelCreating(modelBuilder);

            //// Configure relationships explicitly
            //modelBuilder.Entity<Residence>()
            //    .HasOne(r => r.Location)
            //    .WithMany()
            //    .HasForeignKey(r => r.LocationID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Residence>()
            //    .HasOne(r => r.Client)
            //    .WithMany()
            //    .HasForeignKey(r => r.ClientID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Location>().HasData(
            //    new Location { LocationID = 1, Name = "Denver" },
            //    new Location { LocationID = 2, Name = "Dallas" },
            //    new Location { LocationID = 3, Name = "Detroit" },
            //    new Location { LocationID = 4, Name = "Orlando" },
            //    new Location { LocationID = 5, Name = "Atlanta" },
            //    new Location { LocationID = 6, Name = "Chicago" },
            //    new Location { LocationID = 7, Name = "New York" }
            //);

            //modelBuilder.Entity<Client>().HasData(
            //    new Client
            //    {
            //        ClientID = 1,
            //        Name = "John Doe",
            //        PhoneNumber = "555-0123",
            //        Email = "john@example.com",
            //        SSN = "123-45-6789",
            //        UserType = "Owner",
            //        DOB = new DateTime(1990, 1, 1)
            //    }
            //);

            //modelBuilder.Entity<Residence>().HasData(
            //    new Residence
            //    {
            //        ResidenceID = 1,
            //        Name = "Chicago Loop Apartment",
            //        ResidencePicture = "chicago.jpg",
            //        LocationID = 6,
            //        ClientID = 1,
            //        BuildYear = 2015,
            //        GuestNumber = 4,
            //        BedroomNumber = 2,
            //        BathroomNumber = 1.0m,
            //        PricePerNight = 150.00m
            //    }
            //);

            //modelBuilder.Entity<Reservation>().HasData(
            //    new Reservation
            //    {
            //        ReservationID = 1,
            //        ReservationStartDate = new DateTime(2024, 1, 1),
            //        ReservationEndDate = new DateTime(2024, 1, 5),
            //        ResidenceID = 1
            //    }
            //);

            modelBuilder.ApplyConfiguration(new ConfigureLocation());
            modelBuilder.ApplyConfiguration(new ConfigureReservation());
            modelBuilder.ApplyConfiguration(new ConfigureResidence());
            modelBuilder.ApplyConfiguration(new ConfigureClient());
        }
    }
}