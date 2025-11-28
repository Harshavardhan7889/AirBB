using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirBB.Models
{
    internal class ConfigureLocation : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> entity)
        {
            // seed initial data
            entity.HasData(
                new Location { LocationID = 1, Name = "Denver" },
                new Location { LocationID = 2, Name = "Dallas" },
                new Location { LocationID = 3, Name = "Detroit" },
                new Location { LocationID = 4, Name = "Orlando" },
                new Location { LocationID = 5, Name = "Atlanta" },
                new Location { LocationID = 6, Name = "Chicago" },
                new Location { LocationID = 7, Name = "New York" }
            );
        }
    }

}