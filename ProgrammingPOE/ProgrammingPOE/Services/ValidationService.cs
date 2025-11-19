using ProgrammingPOE.Models;

namespace ProgrammingPOE.Services
{
    public interface IValidationService
    {
        (bool IsValid, string ErrorMessage) ValidateClaim(Claim claim);
        (bool IsValid, string ErrorMessage) ValidateHours(decimal hours);
        (bool IsValid, string ErrorMessage) ValidateRate(decimal rate);
        (bool IsValid, string ErrorMessage) ValidateMonthlyLimit(string userId, decimal amount);
    }

    public class ValidationService : IValidationService
    {
        private const decimal MAX_MONTHLY_AMOUNT = 50000m;
        private const decimal MAX_HOURLY_RATE = 500m;
        private const decimal MAX_HOURS = 160m;

        public (bool IsValid, string ErrorMessage) ValidateClaim(Claim claim)
        {
            // Validate hours
            var hoursValidation = ValidateHours(claim.HoursWorked);
            if (!hoursValidation.IsValid)
                return hoursValidation;

            // Validate rate
            var rateValidation = ValidateRate(claim.HourlyRate);
            if (!rateValidation.IsValid)
                return rateValidation;

            // Validate total amount
            var total = claim.HoursWorked * claim.HourlyRate;
            if (total > MAX_MONTHLY_AMOUNT)
                return (false, $"Total amount R{total} exceeds maximum monthly limit of R{MAX_MONTHLY_AMOUNT}");

            // Validate additional notes (required field)
            if (string.IsNullOrWhiteSpace(claim.AdditionalNotes))
                return (false, "Additional notes are required");

            return (true, "Claim is valid");
        }

        public (bool IsValid, string ErrorMessage) ValidateHours(decimal hours)
        {
            if (hours <= 0)
                return (false, "Hours worked must be greater than 0");

            if (hours > MAX_HOURS)
                return (false, $"Hours worked cannot exceed {MAX_HOURS} hours per month");

            if (hours % 0.5m != 0)
                return (false, "Hours worked must be in 0.5 hour increments");

            return (true, "Hours are valid");
        }

        public (bool IsValid, string ErrorMessage) ValidateRate(decimal rate)
        {
            if (rate <= 0)
                return (false, "Hourly rate must be greater than 0");

            if (rate > MAX_HOURLY_RATE)
                return (false, $"Hourly rate cannot exceed R{MAX_HOURLY_RATE}");

            return (true, "Rate is valid");
        }

        public (bool IsValid, string ErrorMessage) ValidateMonthlyLimit(string userId, decimal amount)
        {
            // In a real app, this would check against database records
            // For now, we'll simulate monthly limit checking
            if (amount > MAX_MONTHLY_AMOUNT)
                return (false, $"Claim amount exceeds monthly limit of R{MAX_MONTHLY_AMOUNT}");

            return (true, "Within monthly limit");
        }

        // Add these methods to ValidationService class
        public (bool IsValid, string ErrorMessage) ValidateForCoordinator(Claim claim)
        {
            // Check if hours are reasonable for academic work
            if (claim.HoursWorked > 80 && claim.TotalAmount > 20000)
            {
                return (false, "High hours and amount detected. Requires additional verification.");
            }

            // Check if hourly rate is within department guidelines
            if (claim.HourlyRate > 300)
            {
                return (false, "Hourly rate exceeds department guidelines. Requires HR approval.");
            }

            // Check for missing supporting documents for high amounts
            if (claim.TotalAmount > 10000)
            {
                // This would check actual documents in a real application
                return (true, "High amount claim - ensure supporting documents are attached.");
            }

            return (true, "Claim meets coordinator approval criteria");
        }

        public (bool IsValid, string ErrorMessage) ValidateBulkClaims(List<Claim> claims)
        {
            var totalAmount = claims.Sum(c => c.TotalAmount);
            if (totalAmount > 50000)
            {
                return (false, "Bulk approval would exceed monthly budget limit");
            }

            var highRiskClaims = claims.Count(c => c.TotalAmount > 10000);
            if (highRiskClaims > 2)
            {
                return (false, "Too many high-value claims in bulk selection");
            }

            return (true, "Bulk approval validation passed");
        }
    }
}