using System.ComponentModel.DataAnnotations;
namespace AirBB.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        public string Name { get; set; } = null!;
    }
}