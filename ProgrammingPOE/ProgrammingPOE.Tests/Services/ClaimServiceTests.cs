using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;
using Xunit;

namespace ProgrammingPOE.Tests.Services
{
    public class ClaimServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly ClaimService _claimService;

        public ClaimServiceTests()
        {
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(m => m.WebRootPath).Returns("wwwroot");
            _claimService = new ClaimService(_mockEnvironment.Object);
        }

        [Fact]
        public void CreateClaim_ValidClaim_ShouldCreateSuccessfully()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerName = "Dr. John Smith",
                HoursWorked = 10.5m,
                HourlyRate = 250.00m,
                AdditionalNotes = "Marking assignments for PROG6212"
            };
            var userId = "user123";
            var files = new List<IFormFile>();

            // Act
            var result = _claimService.CreateClaim(claim, userId, files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dr. John Smith", result.LecturerName);
            Assert.Equal(2625.00m, result.TotalAmount);
            Assert.Equal(ClaimStatus.Submitted, result.Status);
        }

        [Fact]
        public void GetUserClaims_ValidUserId_ShouldReturnUserClaims()
        {
            // Arrange
            var userId = "test-user";
            var claim = new Claim { LecturerName = "Test User", HoursWorked = 5, HourlyRate = 150 };
            _claimService.CreateClaim(claim, userId, new List<IFormFile>());

            // Act
            var result = _claimService.GetUserClaims(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void UpdateClaimStatus_ValidClaimId_ShouldUpdateStatus()
        {
            // Arrange
            var claim = new Claim { LecturerName = "Test Lecturer", HoursWorked = 10, HourlyRate = 100 };
            var createdClaim = _claimService.CreateClaim(claim, "user123", new List<IFormFile>());

            // Act
            _claimService.UpdateClaimStatus(createdClaim.Id, ClaimStatus.Approved, "Manager");
            var updatedClaim = _claimService.GetClaimById(createdClaim.Id);

            // Assert
            Assert.Equal(ClaimStatus.Approved, updatedClaim.Status);
        }
    }
}