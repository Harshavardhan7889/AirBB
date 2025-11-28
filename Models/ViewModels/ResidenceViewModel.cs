using System.ComponentModel.DataAnnotations;

namespace AirBB.Models.ViewModels
{
    public class ResidenceViewModel
    {
        [Required(ErrorMessage = "Please select a location.")]
        public string ActiveWhere { get; set; } = "all";
        [Required(ErrorMessage = "Please select a date range.")]
        public string ActiveWhen { get; set; } = "";
        [Required(ErrorMessage = "Please Enter the number of guests.")]
        public string ActiveWho { get; set; } = "1";
        public Residence Residence { get; set; } = new Residence();
        public List<Location> Locations { get; set; } = new();
        public List<Residence> Residences { get; set; } = new();
        public List<Residence> FilteredResidences { get; set; } = new();
        public Dictionary<int, bool> ResidenceAvailability { get; set; } = new();

        public DateTime? StartDate => ParseStartDate();
        public DateTime? EndDate => ParseEndDate();

        private DateTime? ParseStartDate()
        {
            if (string.IsNullOrEmpty(ActiveWhen)) return null;
            var dates = ActiveWhen.Split(" - ");
            return dates.Length == 2 ? DateTime.TryParse(dates[0], out DateTime date) ? date : null : null;
        }

        private DateTime? ParseEndDate()
        {
            if (string.IsNullOrEmpty(ActiveWhen)) return null;
            var dates = ActiveWhen.Split(" - ");
            return dates.Length == 2 ? DateTime.TryParse(dates[1], out DateTime date) ? date : null : null;
        }
        public string CheckActiveWhere(string id) => string.Equals(ActiveWhere ?? "all", id ?? "all", StringComparison.OrdinalIgnoreCase) ? "active" : "";
    }
}