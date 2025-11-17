using Microsoft.AspNetCore.Mvc;

namespace ProgrammingPOE.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}