using AirBB.Models;
using AirBB.Models.DataLayer;
using AirBB.Models.DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageController : Controller
    {
        private ResidenceContext context;
        private Repository<Reservation> reservationData { get; set; }
        private Repository<Client> clientData { get; set; }
        private Repository<Residence> residenceData { get; set; }
        private Repository<Location> locationData { get; set; }
        public ManageController(ResidenceContext ctx)
        {
            reservationData = new Repository<Reservation>(ctx);
            clientData = new Repository<Client>(ctx);
            residenceData = new Repository<Residence>(ctx);
            locationData = new Repository<Location>(ctx);
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
        public new IActionResult User(Client user, bool IsEdit = false)
        {
            // Convert empty strings to null for nullable fields to avoid database issues
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                user.PhoneNumber = null;
            if (string.IsNullOrWhiteSpace(user.Email))
                user.Email = null;

            if (ModelState.IsValid)
            {
                if (IsEdit && user.ClientID > 0)
                {
                    //var existingUser = context.Clients.Find(user.ClientID);
                    var existingUser = clientData.Get(user.ClientID);
                    if (existingUser != null)
                    {
                        existingUser.Name = user.Name;
                        existingUser.PhoneNumber = user.PhoneNumber;
                        existingUser.Email = user.Email;
                        existingUser.SSN = user.SSN;
                        existingUser.UserType = user.UserType;
                        existingUser.DOB = user.DOB;
                        
                        context.SaveChanges();
                    }
                }
                else
                {
                    clientData.Insert(user);
                    clientData.Save();
                }
                
                return RedirectToAction("User");
            }
            else
            {
                //var users = context.Clients.OrderBy(u => u.Name).ToList();
                var users = clientData.Get(new QueryOptions<Client>
                {
                    OrderBy = b => Guid.NewGuid()
                });
                ViewBag.Users = users;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult GetUser(int id)
        {
            //var client = context.Clients.Find(id);
            var client = clientData.Get(id);
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
            var client = clientData.Get(id);
            //var client = context.Clients.Find(id);
            if (client == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            if (client.UserType == "Owner")
            {
                //var residences = context.Residences
                //    .Include(r => r.Client)
                //    .Where(r => r.Client.ClientID == id)
                //    .ToList();
                var residences = residenceData.List(new QueryOptions<Residence>
                {
                    Includes = "Client",
                    Where = r => r.Client.ClientID == id,
                    OrderBy = r => r.Name,
                    OrderByDirection = "asc"
                });
                var residenceList = residences.ToList();

                if (residenceList.Any())
                {
                    var residenceNames = string.Join(", ", residenceList.Select(r => r.Name));
                    return Json(new 
                    { 
                        success = false, 
                        message = $"Cannot delete owner {client.Name}. Please delete the following residence(s) first: {residenceNames}"
                    });
                }
            }

            //context.Clients.Remove(client);
            clientData.Delete(client);
            clientData.Save();
            
            return Json(new { success = true, message = "User deleted successfully!" });
        }

        [HttpGet]
        public IActionResult Residence()
        {
            // Get all residences including Location and Client
            var residences = residenceData.List(new QueryOptions<Residence>
            {
                Includes = "Location,Client",
                OrderBy = r => r.Name
            });

            var locations = locationData.List(new QueryOptions<Location>
            {
                OrderBy = l => l.Name
            });

            var owners = clientData.List(new QueryOptions<Client>
            {
                Where = c => c.UserType == "Owner",
                OrderBy = c => c.Name
            });

            ViewBag.Residences = residences;
            ViewBag.Locations = locations;
            ViewBag.Owners = owners;

            return View(new Residence());
        }

        [HttpPost]
        public IActionResult Residence(Residence residence, bool IsEdit = false)
        {
            if (ModelState.IsValid)
            {
                if (IsEdit && residence.ResidenceID > 0)
                {
                    var existingResidence = residenceData.Get(residence.ResidenceID);
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

                        residenceData.Update(existingResidence);
                        residenceData.Save();
                    }
                }
                else
                {
                    // Add new
                    residenceData.Insert(residence);
                    residenceData.Save();
                }

                return RedirectToAction("Residence");
            }

            // Validation failed, reload dropdowns
            ViewBag.Residences = residenceData.List(new QueryOptions<Residence>
            {
                Includes = "Location,Client",
                OrderBy = r => r.Name
            });
            ViewBag.Locations = locationData.List(new QueryOptions<Location>
            {
                OrderBy = l => l.Name
            });
            ViewBag.Owners = clientData.List(new QueryOptions<Client>
            {
                Where = c => c.UserType == "Owner",
                OrderBy = c => c.Name
            });

            return View(residence);
        }


        [HttpGet]
        public IActionResult GetResidence(int id)
        {
            var residence = residenceData.Get(new QueryOptions<Residence>
            {
                Includes = "Location,Client",
                Where = r => r.ResidenceID == id
            });

            if (residence == null) return NotFound();

            return Json(new
            {
                residenceID = residence.ResidenceID,
                name = residence.Name ?? "",
                residencePicture = residence.ResidencePicture ?? "",
                locationID = residence.LocationID,
                clientID = residence.ClientID,
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
            var residence = residenceData.Get(new QueryOptions<Residence>
            {
                Includes = "Location,Client",
                Where = r => r.ResidenceID == id
            });

            if (residence == null)
                return Json(new { success = false, message = "Residence not found." });

            var reservations = reservationData.List(new QueryOptions<Reservation>
            {
                Where = r => r.ResidenceID == id
            });

            if (reservations.Any())
            {
                return Json(new
                {
                    success = false,
                    message = $"Cannot delete residence '{residence.Name}'. Please cancel all reservations first."
                });
            }

            residenceData.Delete(residence);
            residenceData.Save();

            return Json(new
            {
                success = true,
                message = $"Residence '{residence.Name}' deleted successfully!"
            });
        }


        [HttpGet]
        public IActionResult Locations()
        {
            var locations = locationData.List(new QueryOptions<Location>
            {
                OrderBy = l => l.Name
            });

            ViewBag.Locations = locations;
            return View(new Location());
        }

        [HttpPost]
        public IActionResult Locations(Location location, bool IsEdit = false)
        {
            // Check duplicate
            var existingLocation = locationData.Get(new QueryOptions<Location>
            {
                Where = l => l.Name.ToLower() == location.Name.ToLower() &&
                             l.LocationID != location.LocationID
            });

            if (existingLocation != null)
                ModelState.AddModelError("Name", "A location with this name already exists.");

            if (ModelState.IsValid)
            {
                if (IsEdit && location.LocationID > 0)
                {
                    var loc = locationData.Get(location.LocationID);
                    if (loc != null)
                    {
                        loc.Name = location.Name;
                        locationData.Update(loc);
                        locationData.Save();
                    }
                }
                else
                {
                    locationData.Insert(location);
                    locationData.Save();
                }

                return RedirectToAction("Locations");
            }

            // Validation failed
            ViewBag.Locations = locationData.List(new QueryOptions<Location>
            {
                OrderBy = l => l.Name
            });

            return View(location);
        }

        [HttpGet]
        public IActionResult GetLocation(int id)
        {
            // Use repository to get the location
            var location = locationData.Get(id);

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
            // Get the location using the repository
            var location = locationData.Get(id);
            if (location == null)
            {
                return Json(new { success = false, message = "Location not found." });
            }

            // Check if the location is being used by any residences
            var residences = residenceData.List(new QueryOptions<Residence>
            {
                //HasWhere = true,
                Where = r => r.LocationID == id,
                Includes = "Location", // optional if you need navigation properties
            });

            if (residences.Any())
            {
                var residenceNames = string.Join(", ", residences.Select(r => r.Name));
                return Json(new
                {
                    success = false,
                    message = $"Cannot delete location '{location.Name}'. Please delete or reassign the following residence(s) first: {residenceNames}"
                });
            }

            // Delete the location
            locationData.Delete(location);
            locationData.Save();

            return Json(new { success = true, message = $"Location '{location.Name}' deleted successfully!" });
        }


        [AcceptVerbs("GET", "POST")]
        public IActionResult ValidateOwner(int clientID)
        {
            if (clientID <= 0)
            {
                return Json(false);
            }

            // Use repository pattern correctly
            var owner = clientData.List(new QueryOptions<Client>
            {
                Where = c => c.ClientID == clientID && c.UserType == "Owner"
            }).FirstOrDefault();

            return Json(owner != null);
        }
    }   
}