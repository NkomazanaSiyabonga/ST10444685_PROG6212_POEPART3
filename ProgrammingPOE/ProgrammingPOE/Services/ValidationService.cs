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
    }
}