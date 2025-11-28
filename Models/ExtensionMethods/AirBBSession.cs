namespace AirBB.Models.ExtensionMethods
{
    public class AirBBSession
    {
        private const string ResidencesKey = "residences";
        private const string CountKey = "reservationcount";
        private const string WhereKey = "where";
        private const string WhenKey = "when";
        private const string WhoKey = "who";

        private ISession session { get; set; }
        public AirBBSession(ISession session) => this.session = session;

        public void SetMyResidences(List<Residence> residences)
        {
            session.SetObject(ResidencesKey, residences);
            session.SetInt32(CountKey, residences.Count);
        }
        public List<Residence> GetMyResidences() =>
            session.GetObject<List<Residence>>(ResidencesKey) ?? new List<Residence>();
        public int? GetMyReservationCount() => session.GetInt32(CountKey);

        public void SetActiveWhere(string location) =>
            session.SetString(WhereKey, location);
        public string GetActiveWhere() =>
            session.GetString(WhereKey) ?? string.Empty;

        public void SetActiveWhen(string? dates)
        {
            if (string.IsNullOrEmpty(dates))
            {
                session.Remove("ActiveWhen");
            }
            else
            {
                session.SetString("ActiveWhen", dates);
            }
        }

        public string? GetActiveWhen()
        {
            return session.GetString("ActiveWhen");
        }

        public void SetActiveWho(string guests) =>
            session.SetString(WhoKey, guests);
        public string GetActiveWho() =>
            session.GetString(WhoKey) ?? string.Empty;

        public void RemoveMyResidences()
        {
            session.Remove(ResidencesKey);
            session.Remove(CountKey);
        }
    }
}