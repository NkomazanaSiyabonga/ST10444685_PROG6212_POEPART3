using ProgrammingPOE.Models;
using Xunit;

namespace ProgrammingPOE.Tests.Models
{
    public class ClaimModelTests
    {
        [Fact]
        public void TotalAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 10.5m,
                HourlyRate = 200.00m
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(2100.00m, totalAmount);
        }

        [Theory]
        [InlineData(5, 100, 500)]
        [InlineData(10.5, 150, 1575)]
        [InlineData(0.5, 50, 25)]
        public void TotalAmount_VariousInputs_ShouldCalculateCorrectly(decimal hours, decimal rate, decimal expected)
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = hours,
                HourlyRate = rate
            };

            // Act
            var result = claim.TotalAmount;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Claim_DefaultStatus_ShouldBeSubmitted()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            Assert.Equal(ClaimStatus.Submitted, claim.Status);
        }
    }
}