using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;

namespace ProgrammingPOE.Controllers
{
    [CustomAuthorization("HRManager")]
    public class HRManagerController : Controller
    {
        private readonly IClaimService _claimService;

        public HRManagerController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            var pendingClaims = _claimService.GetPendingClaims();
            return View(pendingClaims);
        }

        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            _claimService.UpdateClaimStatus(id, ClaimStatus.Approved, "Manager");
            TempData["SuccessMessage"] = "Claim approved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            _claimService.UpdateClaimStatus(id, ClaimStatus.Rejected, "Manager");
            TempData["SuccessMessage"] = "Claim rejected!";
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