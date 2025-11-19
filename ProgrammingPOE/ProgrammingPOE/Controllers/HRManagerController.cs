using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;

namespace ProgrammingPOE.Controllers
{
    [CustomAuthorization("HRManager")]
    public class HRManagerController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IAuthService _authService;

        public HRManagerController(IClaimService claimService, IAuthService authService)
        {
            _claimService = claimService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            var verifiedClaims = _claimService.GetVerifiedClaims();
            return View(verifiedClaims);
        }

        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            var userName = HttpContext.Session.GetString("UserName");
            _claimService.UpdateClaimStatus(id, ClaimStatus.Approved, userName);
            TempData["SuccessMessage"] = "Claim approved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RejectClaim(int id, string rejectionNotes)
        {
            var userName = HttpContext.Session.GetString("UserName");
            _claimService.RejectClaim(id, ClaimStatus.RejectedByHR, userName, rejectionNotes);
            TempData["SuccessMessage"] = "Claim rejected and returned to Coordinator!";
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

        public IActionResult UserManagement()
        {
            try
            {
                var users = _authService.GetAllUsers();
                return View(users);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                TempData["ErrorMessage"] = "Error loading users: " + ex.Message;
                return View(new List<User>());
            }
        }

        [HttpPost]
        public IActionResult CreateUser(string firstName, string lastName, string email, string role)
        {
            try
            {
                var user = _authService.CreateUser(firstName, lastName, email, role);
                TempData["SuccessMessage"] = $"User {firstName} {lastName} created successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("UserManagement");
        }

        [HttpPost]
        public IActionResult UpdateUser(string userId, string firstName, string lastName, string email, string role)
        {
            try
            {
                _authService.UpdateUser(userId, firstName, lastName, email, role);
                TempData["SuccessMessage"] = $"User {firstName} {lastName} updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("UserManagement");
        }

        [HttpPost]
        public IActionResult DeleteUser(string userId)
        {
            try
            {
                _authService.DeleteUser(userId);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("UserManagement");
        }

        public IActionResult Reports()
        {
            return View();
        }
    }
}