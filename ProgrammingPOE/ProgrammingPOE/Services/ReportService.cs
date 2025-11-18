using ProgrammingPOE.Models;

namespace ProgrammingPOE.Services
{
    public interface IReportService
    {
        byte[] GenerateMonthlyReport(List<Claim> claims);
        byte[] GenerateApprovalStatistics(List<Claim> claims);
        byte[] GenerateLecturerReport(List<Claim> claims);
        string GenerateClaimInvoice(Claim claim);
    }

    public class ReportService : IReportService
    {
        public byte[] GenerateMonthlyReport(List<Claim> claims)
        {
            // In a real application, this would generate a PDF using a library like iTextSharp
            // For now, we'll return a simulated PDF
            var reportData = $"Monthly Claims Report\nGenerated: {DateTime.Now}\nTotal Claims: {claims.Count}";
            return System.Text.Encoding.UTF8.GetBytes(reportData);
        }

        public byte[] GenerateApprovalStatistics(List<Claim> claims)
        {
            var approved = claims.Count(c => c.Status == ClaimStatus.Approved);
            var pending = claims.Count(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.Verified);
            var rejected = claims.Count(c => c.Status == ClaimStatus.RejectedByCoordinator || c.Status == ClaimStatus.RejectedByHR);

            var stats = $"Approval Statistics\n" +
                       $"Generated: {DateTime.Now}\n" +
                       $"Total Claims: {claims.Count}\n" +
                       $"Approved: {approved}\n" +
                       $"Pending: {pending}\n" +
                       $"Rejected: {rejected}\n" +
                       $"Approval Rate: {(claims.Count > 0 ? (approved * 100.0 / claims.Count).ToString("F1") : "0")}%";

            return System.Text.Encoding.UTF8.GetBytes(stats);
        }

        public byte[] GenerateLecturerReport(List<Claim> claims)
        {
            var lecturerGroups = claims.GroupBy(c => c.LecturerName)
                                      .Select(g => new { Lecturer = g.Key, Count = g.Count(), Total = g.Sum(c => c.TotalAmount) });

            var report = $"Lecturer Performance Report\nGenerated: {DateTime.Now}\n\n";

            foreach (var group in lecturerGroups)
            {
                report += $"{group.Lecturer}: {group.Count} claims, Total: R{group.Total:F2}\n";
            }

            return System.Text.Encoding.UTF8.GetBytes(report);
        }

        public string GenerateClaimInvoice(Claim claim)
        {
            return $@"
CLAIM INVOICE
=============
Claim ID: {claim.Id}
Lecturer: {claim.LecturerName}
Date: {claim.SubmissionDate:dd MMM yyyy}
Hours Worked: {claim.HoursWorked}
Hourly Rate: R{claim.HourlyRate:F2}
Total Amount: R{claim.TotalAmount:F2}
Status: {claim.Status}
Generated: {DateTime.Now:dd MMM yyyy HH:mm}
";
        }
    }
}