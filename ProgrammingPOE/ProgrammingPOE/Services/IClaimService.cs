using ProgrammingPOE.Models;
using ProgrammingPOE.ViewModels;

namespace ProgrammingPOE.Services
{
    public interface IClaimService
    {
        Claim CreateClaim(Claim claim, string userId, List<IFormFile> files);
        List<Claim> GetUserClaims(string userId);
        List<Claim> GetPendingClaims();
        List<Claim> GetAllClaims();
        Claim GetClaimById(int claimId);
        void UpdateClaimStatus(int claimId, ClaimStatus status, string approvedBy);

        // NEW METHODS FOR WORKFLOW
        List<Claim> GetVerifiedClaims(); // For HR Manager
        List<Claim> GetClaimsForCoordinator(); // Submitted + Returned by HR
        void RejectClaim(int claimId, ClaimStatus status, string rejectedBy, string rejectionNotes);
        void ReturnClaimToLecturer(int claimId, string returnedBy, string correctionNotes);

        DashboardStats GetDashboardStats(string userId);
    }
}