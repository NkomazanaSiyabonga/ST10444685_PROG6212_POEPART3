using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;

namespace ProgrammingPOE.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IClaimService _claimService;

        public ApprovalController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            var allClaims = _claimService.GetAllClaims();
            return View(allClaims);
        }
    }
}