using Microsoft.AspNetCore.Mvc;

namespace ProgrammingPOE.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [CustomAuthorization]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}