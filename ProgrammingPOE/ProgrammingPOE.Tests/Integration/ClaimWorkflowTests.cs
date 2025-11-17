using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;
using Xunit;

namespace ProgrammingPOE.Tests.Integration
{
    public class ClaimWorkflowTests
    {
        private readonly ClaimService _claimService;
        private readonly AuthService _authService;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;

        public ClaimWorkflowTests()
        {
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(m => m.WebRootPath).Returns("wwwroot");
            _claimService = new ClaimService(_mockEnvironment.Object);
            _authService = new AuthService();
        }

        [Fact]
        public void FullClaimWorkflow_ShouldWorkCorrectly()
        {
            // Arrange - Register user
            var user = _authService.Register("Workflow", "Test", "workflow@test.com", "password123", "Lecturer");

            // Act 1 - Create claim
            var claim = new Claim
            {
                LecturerName = "Workflow Test",
                HoursWorked = 15.5m,
                HourlyRate = 180.00m,
                AdditionalNotes = "Complete workflow test"
            };

            var createdClaim = _claimService.CreateClaim(claim, user.Id, new List<IFormFile>());

            // Assert 1 - Claim created
            Assert.Equal(ClaimStatus.Submitted, createdClaim.Status);

            // Act 2 - Coordinator verifies
            _claimService.UpdateClaimStatus(createdClaim.Id, ClaimStatus.Verified, "Coordinator");
            var verifiedClaim = _claimService.GetClaimById(createdClaim.Id);

            // Assert 2 - Claim verified
            Assert.Equal(ClaimStatus.Verified, verifiedClaim.Status);

            // Act 3 - Manager approves
            _claimService.UpdateClaimStatus(createdClaim.Id, ClaimStatus.Approved, "Manager");
            var approvedClaim = _claimService.GetClaimById(createdClaim.Id);

            // Assert 3 - Claim approved
            Assert.Equal(ClaimStatus.Approved, approvedClaim.Status);
        }

        [Fact]
        public void ClaimRejectionWorkflow_ShouldWorkCorrectly()
        {
            // Arrange
            var user = _authService.Register("Rejection", "Test", "reject@test.com", "pass123", "Lecturer");
            var claim = new Claim { LecturerName = "Rejection Test", HoursWorked = 10, HourlyRate = 100 };
            var createdClaim = _claimService.CreateClaim(claim, user.Id, new List<IFormFile>());

            // Act - Reject claim
            _claimService.UpdateClaimStatus(createdClaim.Id, ClaimStatus.Rejected, "Coordinator");
            var rejectedClaim = _claimService.GetClaimById(createdClaim.Id);

            // Assert
            Assert.Equal(ClaimStatus.Rejected, rejectedClaim.Status);
        }
    }
}