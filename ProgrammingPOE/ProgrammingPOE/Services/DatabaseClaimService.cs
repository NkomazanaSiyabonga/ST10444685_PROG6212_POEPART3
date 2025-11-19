using Microsoft.EntityFrameworkCore;
using ProgrammingPOE.Data;
using ProgrammingPOE.Models;
using ProgrammingPOE.ViewModels;

namespace ProgrammingPOE.Services
{
    public class DatabaseClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DatabaseClaimService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public Claim CreateClaim(Claim claim, string userId, List<IFormFile> files)
        {
            claim.UserId = userId;
            claim.SubmissionDate = DateTime.Now;
            claim.Status = ClaimStatus.Submitted;

            // EXPLICITLY SET APPROVAL FIELDS TO NULL FOR NEW CLAIMS
            claim.ApprovedBy = null;
            claim.VerifiedBy = null;
            claim.RejectedBy = null;
            claim.RejectionNotes = null;
            claim.VerificationDate = null;
            claim.ApprovalDate = null;

            _context.Claims.Add(claim);
            _context.SaveChanges();

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    if (file.Length > 0 && file.Length < 5 * 1024 * 1024)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var document = new SupportingDocument
                        {
                            ClaimId = claim.Id,
                            FileName = file.FileName,
                            FilePath = uniqueFileName,
                            ContentType = file.ContentType,
                            FileSize = file.Length,
                            UploadDate = DateTime.Now
                        };

                        _context.SupportingDocuments.Add(document);
                    }
                }
                _context.SaveChanges();
            }

            return claim;
        }

        public List<Claim> GetUserClaims(string userId)
        {
            return _context.Claims
                .Include(c => c.SupportingDocuments)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.SubmissionDate)
                .ToList();
        }

        public List<Claim> GetPendingClaims()
        {
            return _context.Claims
                .Include(c => c.SupportingDocuments)
                .Where(c => c.Status == ClaimStatus.Submitted)
                .OrderBy(c => c.SubmissionDate)
                .ToList();
        }

        public List<Claim> GetAllClaims()
        {
            return _context.Claims
                .Include(c => c.SupportingDocuments)
                .OrderByDescending(c => c.SubmissionDate)
                .ToList();
        }

        public Claim GetClaimById(int claimId)
        {
            return _context.Claims
                .Include(c => c.SupportingDocuments)
                .FirstOrDefault(c => c.Id == claimId);
        }

        public void UpdateClaimStatus(int claimId, ClaimStatus status, string approvedBy)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.Status = status;

                if (status == ClaimStatus.Verified)
                {
                    claim.VerifiedBy = approvedBy;
                    claim.VerificationDate = DateTime.Now;
                }
                else if (status == ClaimStatus.Approved)
                {
                    claim.ApprovedBy = approvedBy;
                    claim.ApprovalDate = DateTime.Now;
                }

                _context.SaveChanges();
            }
        }

        public List<Claim> GetVerifiedClaims()
        {
            return _context.Claims
                .Include(c => c.SupportingDocuments)
                .Where(c => c.Status == ClaimStatus.Verified)
                .OrderBy(c => c.SubmissionDate)
                .ToList();
        }

        public List<Claim> GetClaimsForCoordinator()
        {
            return _context.Claims
                .Include(c => c.SupportingDocuments)
                .Where(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.RejectedByHR)
                .OrderBy(c => c.SubmissionDate)
                .ToList();
        }

        public void RejectClaim(int claimId, ClaimStatus status, string rejectedBy, string rejectionNotes)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.Status = status;
                claim.RejectedBy = rejectedBy;
                claim.RejectionNotes = rejectionNotes;
                _context.SaveChanges();
            }
        }

        public void ReturnClaimToLecturer(int claimId, string returnedBy, string correctionNotes)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.Status = ClaimStatus.ReturnedForCorrection;
                claim.RejectedBy = returnedBy;
                claim.RejectionNotes = correctionNotes;
                _context.SaveChanges();
            }
        }

        public DashboardStats GetDashboardStats(string userId)
        {
            var userClaims = _context.Claims.Where(c => c.UserId == userId).ToList();

            return new DashboardStats
            {
                TotalClaims = userClaims.Count,
                PendingClaims = userClaims.Count(c => c.Status == ClaimStatus.Submitted),
                ApprovedClaims = userClaims.Count(c => c.Status == ClaimStatus.Approved),
                RejectedClaims = userClaims.Count(c => c.Status == ClaimStatus.RejectedByCoordinator || c.Status == ClaimStatus.RejectedByHR),
                TotalAmount = userClaims.Where(c => c.Status == ClaimStatus.Approved).Sum(c => c.TotalAmount)
            };
        }
    }
}