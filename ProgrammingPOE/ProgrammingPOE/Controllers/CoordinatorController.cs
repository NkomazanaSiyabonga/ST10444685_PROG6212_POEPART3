using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;

namespace ProgrammingPOE.Controllers
{
    [CustomAuthorization("Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly IClaimService _claimService;

        public CoordinatorController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            // Coordinator sees submitted claims and claims returned by HR
            var pendingClaims = _claimService.GetClaimsForCoordinator();
            return View(pendingClaims);
        }

        [HttpPost]
        public IActionResult VerifyClaim(int id)
        {
            var userName = HttpContext.Session.GetString("UserName");
            _claimService.UpdateClaimStatus(id, ClaimStatus.Verified, userName);
            TempData["SuccessMessage"] = "Claim verified successfully! Sent to HR Manager for approval.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RejectClaim(int id, string rejectionNotes)
        {
            var userName = HttpContext.Session.GetString("UserName");
            _claimService.RejectClaim(id, ClaimStatus.RejectedByCoordinator, userName, rejectionNotes);
            TempData["SuccessMessage"] = "Claim rejected! Lecturer has been notified.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ReturnToLecturer(int id, string correctionNotes)
        {
            var userName = HttpContext.Session.GetString("UserName");
            _claimService.ReturnClaimToLecturer(id, userName, correctionNotes);
            TempData["SuccessMessage"] = "Claim returned to lecturer for corrections.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var claim = _claimService.GetClaimById(id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }
    }
}