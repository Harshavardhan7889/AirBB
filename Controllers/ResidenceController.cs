using Microsoft.AspNetCore.Mvc;

namespace AirBB.Controllers
{
    public class ResidenceController : Controller
    {
        public IActionResult List(string id = "All")
        {
            
            return View();
        }
    }
}