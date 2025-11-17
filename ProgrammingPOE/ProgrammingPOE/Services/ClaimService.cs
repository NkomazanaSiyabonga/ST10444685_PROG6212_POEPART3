using ProgrammingPOE.Models;
using ProgrammingPOE.ViewModels;

namespace ProgrammingPOE.Services
{
    public class ClaimService : IClaimService
    {
        private static List<Claim> _claims = new List<Claim>();
        private static List<SupportingDocument> _documents = new List<SupportingDocument>();
        private static int _nextClaimId = 1;
        private static int _nextDocumentId = 1;
        private readonly IWebHostEnvironment _environment;

        public ClaimService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public Claim CreateClaim(Claim claim, string userId, List<IFormFile> files)
        {
            claim.Id = _nextClaimId++;
            claim.UserId = userId;
            claim.SubmissionDate = DateTime.Now;
            claim.Status = ClaimStatus.Submitted;

            _claims.Add(claim);

            // Handle file uploads
            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    if (file.Length > 0 && file.Length < 5 * 1024 * 1024) // 5MB limit
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
                            Id = _nextDocumentId++,
                            ClaimId = claim.Id,
                            FileName = file.FileName,
                            FilePath = uniqueFileName,
                            ContentType = file.ContentType,
                            FileSize = file.Length,
                            UploadDate = DateTime.Now
                        };

                        _documents.Add(document);
                    }
                }
            }

            return claim;
        }

        public List<Claim> GetUserClaims(string userId)
        {
            var claims = _claims
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.SubmissionDate)
                .ToList();

            // Attach documents to each claim
            foreach (var claim in claims)
            {
                claim.SupportingDocuments = _documents.Where(d => d.ClaimId == claim.Id).ToList();
            }

            return claims;
        }

        public List<Claim> GetPendingClaims()
        {
            var claims = _claims
                .Where(c => c.Status == ClaimStatus.Submitted)
                .OrderBy(c => c.SubmissionDate)
                .ToList();

            // Attach documents to each claim
            foreach (var claim in claims)
            {
                claim.SupportingDocuments = _documents.Where(d => d.ClaimId == claim.Id).ToList();
            }

            return claims;
        }

        public List<Claim> GetAllClaims()
        {
            var claims = _claims
                .OrderByDescending(c => c.SubmissionDate)
                .ToList();

            // Attach documents to each claim
            foreach (var claim in claims)
            {
                claim.SupportingDocuments = _documents.Where(d => d.ClaimId == claim.Id).ToList();
            }

            return claims;
        }

        public Claim GetClaimById(int claimId)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.SupportingDocuments = _documents.Where(d => d.ClaimId == claimId).ToList();
            }
            return claim;
        }

        public void UpdateClaimStatus(int claimId, ClaimStatus status, string approvedBy)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.Status = status;
            }
        }

        public DashboardStats GetDashboardStats(string userId)
        {
            var userClaims = _claims.Where(c => c.UserId == userId).ToList();

            return new DashboardStats
            {
                TotalClaims = userClaims.Count,
                PendingClaims = userClaims.Count(c => c.Status == ClaimStatus.Submitted),
                ApprovedClaims = userClaims.Count(c => c.Status == ClaimStatus.Approved),
                RejectedClaims = userClaims.Count(c => c.Status == ClaimStatus.Rejected),
                TotalAmount = userClaims.Where(c => c.Status == ClaimStatus.Approved).Sum(c => c.TotalAmount)
            };
        }
    }
}