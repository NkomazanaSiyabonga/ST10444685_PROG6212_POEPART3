using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;
using ProgrammingPOE.ViewModels;

namespace ProgrammingPOE.Controllers
{
    [CustomAuthorization("Lecturer")]
    public class LecturerController : Controller
    {
        private readonly IClaimService _claimService;

        public LecturerController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            var model = new ClaimViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitClaim(string LecturerName, decimal HoursWorked, decimal HourlyRate, string AdditionalNotes, List<IFormFile> supportingDocuments)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                // Debug: Check what values are coming in
                Console.WriteLine($"LecturerName: {LecturerName}");
                Console.WriteLine($"HoursWorked: {HoursWorked}");
                Console.WriteLine($"HourlyRate: {HourlyRate}");
                Console.WriteLine($"AdditionalNotes: {AdditionalNotes}");
                Console.WriteLine($"Files: {supportingDocuments?.Count ?? 0}");

                var claim = new Claim
                {
                    LecturerName = LecturerName,
                    HoursWorked = HoursWorked,
                    HourlyRate = HourlyRate,
                    AdditionalNotes = AdditionalNotes // This can be null/empty
                };

                _claimService.CreateClaim(claim, userId, supportingDocuments);
                return RedirectToAction("MyClaims");
            }
            catch (Exception ex)
            {
                // If there's an error, return to the form with error message
                ViewBag.Error = $"Error submitting claim: {ex.Message}";
                return View("Index");
            }
        }

        public IActionResult MyClaims()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            // FIXED: Changed from GetLecturerClaims to GetUserClaims
            var claims = _claimService.GetUserClaims(userId);
            return View(claims);
        }
    }
}