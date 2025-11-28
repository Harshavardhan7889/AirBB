using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirBB.Models
{
    internal class ConfigureClient : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            // seed initial data
            entity.HasData(
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
        }
    }

}