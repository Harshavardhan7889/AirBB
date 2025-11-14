using System.ComponentModel.DataAnnotations;
namespace AirBB.Models
{
    public class Location
    {
        public string LocationID { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}