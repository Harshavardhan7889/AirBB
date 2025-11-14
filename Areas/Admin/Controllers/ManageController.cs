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
                    // Edit existing user
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
                        TempData["SuccessMessage"] = "User updated successfully!";
                    }
                }
                else
                {
                    // Add new user
                    context.Clients.Add(client);
                    context.SaveChanges();
                    TempData["SuccessMessage"] = "User added successfully!";
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
                dob = client.DOB?.ToString("yyyy-MM-dd") ?? "" // Handle null DOB
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

            // Check if user is an owner with residences
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

            // Safe to delete
            context.Clients.Remove(client);
            context.SaveChanges();
            
            return Json(new { success = true, message = "User deleted successfully!" });
        }
    }   
}