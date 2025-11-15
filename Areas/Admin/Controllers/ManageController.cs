using AirBB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageController : Controller
    {
        private ResidenceContext context;
        
        public ManageController(ResidenceContext ctx)
        {
            context = ctx;
        }
        
        [HttpGet]    
        public IActionResult Index() => View();

        [HttpGet]
        public new IActionResult User()
        {
            var users = context.Clients.OrderBy(c => c.Name).ToList();
            ViewBag.Users = users;
            return View(new Client());
        }

        [HttpPost]
        public new IActionResult User(Client client, bool IsEdit = false)
        {
            if (ModelState.IsValid)
            {
                if (IsEdit && client.ClientID > 0)
                {
                    var existingClient = context.Clients.Find(client.ClientID);
                    if (existingClient != null)
                    {
                        existingClient.Name = client.Name;
                        existingClient.PhoneNumber = client.PhoneNumber;
                        existingClient.Email = client.Email;
                        existingClient.SSN = client.SSN;
                        existingClient.UserType = client.UserType;
                        existingClient.DOB = client.DOB;
                        
                        context.SaveChanges();
                    }
                }
                else
                {
                    context.Clients.Add(client);
                    context.SaveChanges();
                }
                
                return RedirectToAction("User");
            }
            else
            {
                var users = context.Clients.OrderBy(c => c.Name).ToList();
                ViewBag.Users = users;
                return View(client);
            }
        }

        [HttpGet]
        public IActionResult GetUser(int id)
        {
            var client = context.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                clientID = client.ClientID,
                name = client.Name,
                phoneNumber = client.PhoneNumber,
                email = client.Email,
                ssn = client.SSN,
                userType = client.UserType,
                dob = client.DOB?.ToString("yyyy-MM-dd") ?? ""
            });
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var client = context.Clients.Find(id);
            if (client == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            if (client.UserType == "Owner")
            {
                var residences = context.Residences
                    .Include(r => r.Client)
                    .Where(r => r.Client.ClientID == id)
                    .ToList();

                if (residences.Any())
                {
                    var residenceNames = string.Join(", ", residences.Select(r => r.Name));
                    return Json(new 
                    { 
                        success = false, 
                        message = $"Cannot delete owner {client.Name}. Please delete the following residence(s) first: {residenceNames}"
                    });
                }
            }

            context.Clients.Remove(client);
            context.SaveChanges();
            
            return Json(new { success = true, message = "User deleted successfully!" });
        }

        [HttpGet]
        public IActionResult Residence()
        {
            var residences = context.Residences
                .Include(r => r.Location)
                .Include(r => r.Client)
                .OrderBy(r => r.Name)
                .ToList();
            
            var locations = context.Locations.OrderBy(l => l.Name).ToList();
            var owners = context.Clients.Where(c => c.UserType == "Owner").OrderBy(c => c.Name).ToList();
            
            ViewBag.Residences = residences;
            ViewBag.Locations = locations;
            ViewBag.Owners = owners;
            
            return View(new Residence());
        }

        [HttpPost]
        public IActionResult Residence(Residence residence, bool IsEdit = false)
        {
            // Remove the manual LocationID and ClientID checks since they're now part of the model
    
            if (ModelState.IsValid)
            {
                if (IsEdit && residence.ResidenceID > 0)
                {
                    // Edit existing residence
                    var existingResidence = context.Residences
                        .FirstOrDefault(r => r.ResidenceID == residence.ResidenceID);
                    
                    if (existingResidence != null)
                    {
                        // Update all properties
                        existingResidence.Name = residence.Name;
                        existingResidence.ResidencePicture = residence.ResidencePicture;
                        existingResidence.BuildYear = residence.BuildYear;
                        existingResidence.GuestNumber = residence.GuestNumber;
                        existingResidence.BedroomNumber = residence.BedroomNumber;
                        existingResidence.BathroomNumber = residence.BathroomNumber;
                        existingResidence.PricePerNight = residence.PricePerNight;
                        existingResidence.LocationID = residence.LocationID;
                        existingResidence.ClientID = residence.ClientID;
                        
                        context.SaveChanges();
                    }
                }
                else
                {
                    // Add new residence
                    context.Residences.Add(residence);
                    context.SaveChanges();
                }
                
                return RedirectToAction("Residence");
            }
            else
            {
                // Reload data for dropdowns in case of validation errors
                var residences = context.Residences
                    .Include(r => r.Location)
                    .Include(r => r.Client)
                    .OrderBy(r => r.Name)
                    .ToList();
                
                var locations = context.Locations.OrderBy(l => l.Name).ToList();
                var owners = context.Clients.Where(c => c.UserType == "Owner").OrderBy(c => c.Name).ToList();
                
                ViewBag.Residences = residences;
                ViewBag.Locations = locations;
                ViewBag.Owners = owners;
                
                return View(residence);
            }
        }

        [HttpGet]
        public IActionResult GetResidence(int id)
        {
            var residence = context.Residences
                .Include(r => r.Location)
                .Include(r => r.Client)
                .FirstOrDefault(r => r.ResidenceID == id);
    
            if (residence == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                residenceID = residence.ResidenceID,
                name = residence.Name ?? "",
                residencePicture = residence.ResidencePicture ?? "",
                locationID = residence.LocationID, // Use the foreign key property
                clientID = residence.ClientID,     // Use the foreign key property
                buildYear = residence.BuildYear,
                guestNumber = residence.GuestNumber,
                bedroomNumber = residence.BedroomNumber,
                bathroomNumber = residence.BathroomNumber,
                pricePerNight = residence.PricePerNight
            });
        }

        [HttpPost]
        public IActionResult DeleteResidence(int id)
        {
            var residence = context.Residences
                .Include(r => r.Location)
                .Include(r => r.Client)
                .FirstOrDefault(r => r.ResidenceID == id);
                
            if (residence == null)
            {
                return Json(new { success = false, message = "Residence not found." });
            }

            // Check if residence has reservations
            var reservations = context.Reservations.Where(r => r.ResidenceID == id).ToList();
            if (reservations.Any())
            {
                return Json(new 
                { 
                    success = false, 
                    message = $"Cannot delete residence '{residence.Name}'. Please cancel all reservations first."
                });
            }

            context.Residences.Remove(residence);
            context.SaveChanges();
            
            return Json(new { success = true, message = $"Residence '{residence.Name}' deleted successfully!" });
        }

        [HttpGet]
        public IActionResult Locations()
        {
            var locations = context.Locations.OrderBy(l => l.Name).ToList();
            ViewBag.Locations = locations;
            return View(new Location());
        }

        [HttpPost]
        public IActionResult Locations(Location location, bool IsEdit = false)
        {
            // Check for duplicate location name (exclude current location if editing)
            var existingLocation = context.Locations
                .Where(l => l.Name.ToLower() == location.Name.ToLower() && l.LocationID != location.LocationID)
                .FirstOrDefault();
            
            if (existingLocation != null)
            {
                ModelState.AddModelError("Name", "A location with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                if (IsEdit && location.LocationID > 0)
                {
                    // Edit existing location
                    var existingLoc = context.Locations.Find(location.LocationID);
                    if (existingLoc != null)
                    {
                        existingLoc.Name = location.Name;
                        context.SaveChanges();
                    }
                }
                else
                {
                    // Add new location
                    context.Locations.Add(location);
                    context.SaveChanges();
                }
                
                return RedirectToAction("Locations");
            }
            else
            {
                // Reload data in case of validation errors
                var locations = context.Locations.OrderBy(l => l.Name).ToList();
                ViewBag.Locations = locations;
                return View(location);
            }
        }

        [HttpGet]
        public IActionResult GetLocation(int id)
        {
            var location = context.Locations.Find(id);
            if (location == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                locationID = location.LocationID,
                name = location.Name
            });
        }

        [HttpPost]
        public IActionResult DeleteLocation(int id)
        {
            var location = context.Locations.Find(id);
            if (location == null)
            {
                return Json(new { success = false, message = "Location not found." });
            }

            // Check if location is being used by any residences
            var residences = context.Residences
                .Include(r => r.Location)
                .Where(r => r.LocationID == id)
                .ToList();

            if (residences.Any())
            {
                var residenceNames = string.Join(", ", residences.Select(r => r.Name));
                return Json(new 
                { 
                    success = false, 
                    message = $"Cannot delete location '{location.Name}'. Please delete or reassign the following residence(s) first: {residenceNames}"
                });
            }

            context.Locations.Remove(location);
            context.SaveChanges();
            
            return Json(new { success = true, message = $"Location '{location.Name}' deleted successfully!" });
        }
    }   
}