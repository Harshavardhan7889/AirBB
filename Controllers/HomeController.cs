using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AirBB.Models;
using Microsoft.EntityFrameworkCore;
using AirBB.Models.ViewModels;
using AirBB.Models.DataLayer;
using AirBB.Models.ExtensionMethods;
using AirBB.Models.DataLayer.Repositories;

namespace AirBB.Controllers;

public class HomeController : Controller
{
    private ResidenceContext context;
    private Repository<Residence> data { get; set; }
    public HomeController(ResidenceContext ctx)
    {
        data = new Repository<Residence>(ctx);
        context = ctx;
    }
    //public HomeController(ResidenceContext ctx) => context = ctx;
    public ViewResult Index(ResidenceViewModel model)
    {
        model ??= new ResidenceViewModel();
        var session = new AirBBSession(HttpContext.Session);
        if (!string.IsNullOrEmpty(model.ActiveWhen))
            session.SetActiveWhen(model.ActiveWhen);
        else
            session.SetActiveWhen("");

        session.SetActiveWhere(model.ActiveWhere);
        session.SetActiveWho(model.ActiveWho);

        int? count = session.GetMyReservationCount();
        if (!count.HasValue)
        {
            var cookies = new AirBBCookies(Request.Cookies);
            string[] ids = cookies.GetMyResidenceIds();

            if (ids.Length > 0)
            {
                var myresidences = context.Residences
                    .Include(t => t.Location)
                    .Where(t => ids.Contains(t.ResidenceID.ToString()))
                    .ToList();
                session.SetMyResidences(myresidences);
            }
        }

        model.Locations = context.Locations.ToList();
        // Initial query
        IQueryable<Residence> query = context.Residences
            .Include(r => r.Location)
            .OrderBy(r => r.Name);

        // Filter by location
        if (model.ActiveWhere != "all")
        {
            query = query.Where(r => r.Location.LocationID.ToString().ToLower() ==
                model.ActiveWhere.ToLower());
        }

        // Filter by guests
        if (!string.IsNullOrEmpty(model.ActiveWho) &&
            int.TryParse(model.ActiveWho, out int guests))
        {
            query = query.Where(r => r.GuestNumber >= guests);
        }
        
        model.FilteredResidences = query.ToList();

        // Check availability for each residence if dates are selected
        if (!string.IsNullOrEmpty(model.ActiveWhen) && model.StartDate.HasValue && model.EndDate.HasValue)
        {
            var startDate = model.StartDate.Value;
            var endDate = model.EndDate.Value;

            foreach (var residence in model.FilteredResidences)
            {
                // Check if the residence has any overlapping reservations
                bool isAvailable = !context.Reservations.Any(r => 
                    r.ResidenceID == residence.ResidenceID && 
                    r.ReservationStartDate <= endDate && 
                    r.ReservationEndDate >= startDate);
                
                model.ResidenceAvailability[residence.ResidenceID] = isAvailable;
            }
        }
        else
        {
            // If no dates selected, mark all as available
            foreach (var residence in model.FilteredResidences)
            {
                model.ResidenceAvailability[residence.ResidenceID] = true;
            }
        }
        
        return View(model);
    }

    [HttpGet]
    public IActionResult Details(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToAction("Index");

        var residence = context.Residences
            .Include(r => r.Location)
            .FirstOrDefault(r => r.ResidenceID.ToString() == id);

        if (residence == null)
            return NotFound(); 
        var session = new AirBBSession(HttpContext.Session);
        var model = new ResidenceViewModel
        {
            Residence = context.Residences
                .Include(r => r.Location)
                .FirstOrDefault(r => r.ResidenceID.ToString() == id) ?? new Residence(),
            ActiveWhere = session.GetActiveWhere(),
            ActiveWhen = session.GetActiveWhen() ?? string.Empty,
            ActiveWho = session.GetActiveWho()
        };
        return View(model);
    }
}

