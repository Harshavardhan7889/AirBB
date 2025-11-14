namespace AirBB.Models
{
    public class AirBBCookies
    {
        private const string ResidencesKey = "myteams";
        private const string Delimiter = "-";

        private IRequestCookieCollection requestCookies { get; set; } = null!;
        private IResponseCookies responseCookies { get; set; } = null!;

        public AirBBCookies(IRequestCookieCollection cookies) 
        {
            requestCookies = cookies;
            responseCookies = null!;
        }
        public AirBBCookies(IResponseCookies cookies) 
        {
            responseCookies = cookies;
            requestCookies = null!;
        }

        public void SetMyResidenceIds(List<Residence> myresidences)
        {
            List<string> ids = myresidences.Select(t => t.ResidenceID).ToList();
            string idsString = String.Join(Delimiter, ids);
            CookieOptions options = new CookieOptions { 
                Expires = DateTime.Now.AddDays(7) 
            };
            RemoveMyResidenceIds();     // delete old cookie first
            responseCookies.Append(ResidencesKey, idsString, options);
        }

        public string[] GetMyResidenceIds()
        {
            string cookie = requestCookies[ResidencesKey] ?? String.Empty;
            if (string.IsNullOrEmpty(cookie))
                return Array.Empty<string>();
                return cookie.Split(Delimiter);
        }      

        public void RemoveMyResidenceIds()
        {
            responseCookies.Delete(ResidencesKey);
        }
    }
}
