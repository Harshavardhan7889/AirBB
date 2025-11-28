using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;
using AirBB.Models.DataLayer;
using AirBB.Models.ViewModels;
using AirBB.Models.ExtensionMethods;
using AirBB.Models.DataLayer.Repositories;

namespace AirBB.Controllers
{
    public class ReservationsController : Controller
    {
        private ResidenceContext context;
        private Repository<Reservation> data { get; set; }
        public ReservationsController(ResidenceContext ctx)
        {
            data = new Repository<Reservation>(ctx);
            context = ctx;
        }

        [HttpGet]
        public ViewResult Index()
        {
            var session = new AirBBSession(HttpContext.Session);
            var model = new ResidenceViewModel
            {
                ActiveWhere = session.GetActiveWhere(),
                ActiveWho = session.GetActiveWho(),
                Residences = session.GetMyResidences()
            };

            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult Add(Residence residence)
        {
            // get full residence data from database
            residence = context.Residences
                    .Include(r => r.Location)
                    .FirstOrDefault(r => r.ResidenceID == residence.ResidenceID) ?? new Residence();

            // Get dates from session
            var session = new AirBBSession(HttpContext.Session);
            var dates = session.GetActiveWhen()?.Split(" - ");

            if (dates?.Length != 2 ||
                !DateTime.TryParse(dates[0], out DateTime startDate) ||
                !DateTime.TryParse(dates[1], out DateTime endDate))
            {
                TempData["message"] = "Please select valid dates before reserving.";
                return RedirectToAction("Details", "Home", new { id = residence.ResidenceID });
            }

            // Create and save the reservation to database
            var reservation = new Reservation
            {
                ResidenceID = residence.ResidenceID,
                ReservationStartDate = startDate,
                ReservationEndDate = endDate
            };

            data.Insert(reservation);
            data.Save();

            // add residence to favorite residences in session and cookie
            var cookies = new AirBBCookies(Response.Cookies);
            var residences = session.GetMyResidences() ?? new List<Residence>();
            residences.Add(residence);
            session.SetMyResidences(residences);
            cookies.SetMyResidenceIds(residences);

            // set confirmation message
            TempData["message"] = $"Reservation confirmed for {residence.Name}";

            // redirect to Home page
            return RedirectToAction("Index", "Home",
                new
                {
                    ActiveWhere = session.GetActiveWhere(),
                    ActiveWhen = session.GetActiveWhen(),
                    ActiveWho = session.GetActiveWho()
                });
        }

        [HttpPost]
        public IActionResult Cancel(string residenceId)
        {
            if (string.IsNullOrEmpty(residenceId))
                return RedirectToAction("Index", "Reservations");

            // Find and remove all reservations for this residence
            var reservations = context.Reservations
                .Where(r => r.Residence.ResidenceID.ToString() == residenceId)
                .ToList();

            if (reservations.Any())
            {
                context.Reservations.RemoveRange(reservations);
                context.SaveChanges();
            }

            // Update session-stored reservations
            var session = new AirBBSession(HttpContext.Session);
            var myRes = session.GetMyResidences() ?? new List<Residence>();
            myRes = myRes.Where(r => r.ResidenceID.ToString() != residenceId).ToList();
            session.SetMyResidences(myRes);

            // Update cookie-stored ids
            var cookies = new AirBBCookies(Response.Cookies);
            cookies.SetMyResidenceIds(myRes);

            TempData["Message"] = "Reservation cancelled successfully.";
            return RedirectToAction("Index", "Reservations");
        }
        
        [HttpPost]
        public RedirectToActionResult Delete()
        {
            // delete favorite residences from session and cookie
            var session = new AirBBSession(HttpContext.Session);
            var cookies = new AirBBCookies(Response.Cookies);

            session.RemoveMyResidences();
            cookies.RemoveMyResidenceIds();

            // set delete message
            TempData["message"] = "Reserved residences cleared";

            // redirect to Home page
            return RedirectToAction("Index", "Home",
                new {
                    ActiveWhere = session.GetActiveWhere(),
                    ActiveWhen = session.GetActiveWhen(),
                    ActiveWho = session.GetActiveWho()
                });
        }
    }
}
    