using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Services;

namespace ProgrammingPOE.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IClaimService _claimService;

        public DashboardController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            // For demo - in real app this would come from session
            var lecturerName = "Demo Lecturer";
            var stats = _claimService.GetDashboardStats(lecturerName);
            return View(stats);
        }
    }
}