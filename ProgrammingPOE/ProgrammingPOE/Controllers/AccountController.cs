using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Services;

namespace ProgrammingPOE.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string role)
        {
            try
            {
                // Remove role validation - just pass the credentials
                var user = _authService.Login(email, password, role);

                // Store user info in session
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("UserEmail", user.Email);

                TempData["SuccessMessage"] = $"Welcome back, {user.FirstName}!";
                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Role = role;
                ViewBag.Email = email;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password, string confirmPassword, string role)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                    string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(role))
                {
                    ViewBag.Error = "All fields are required.";
                    return View();
                }

                if (password != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    return View();
                }

                if (password.Length < 6)
                {
                    ViewBag.Error = "Password must be at least 6 characters long.";
                    return View();
                }

                var user = _authService.Register(firstName, lastName, email, password, role);

                // Auto-login after registration
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("UserEmail", user.Email);

                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}