using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;
using ProgrammingPOE.ViewModels;

namespace ProgrammingPOE.Controllers
{
    [CustomAuthorization("Lecturer")]
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;

        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClaimViewModel claimModel, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("UserId");

                var claim = new Claim
                {
                    LecturerName = claimModel.LecturerName,
                    HoursWorked = claimModel.HoursWorked,
                    HourlyRate = claimModel.HourlyRate,
                    AdditionalNotes = claimModel.AdditionalNotes
                };

                _claimService.CreateClaim(claim, userId, files);

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }
            return View(claimModel);
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var claims = _claimService.GetUserClaims(userId);
            return View(claims);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = _claimService.GetClaimById(id.Value);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }
    }
}