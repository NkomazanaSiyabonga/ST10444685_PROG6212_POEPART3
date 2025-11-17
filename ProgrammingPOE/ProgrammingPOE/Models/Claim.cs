using System.ComponentModel.DataAnnotations;

namespace ProgrammingPOE.Models
{
    public enum ClaimStatus
    {
        Submitted,      // Lecturer submitted
        Verified,       // Coordinator verified
        Approved,       // HR Manager approved
        RejectedByCoordinator,  // Coordinator rejected
        RejectedByHR,   // HR Manager rejected
        ReturnedForCorrection  // Sent back to lecturer
    }

    public class Claim
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Required]
        [Display(Name = "Hours Worked")]
        [Range(0.5, 160, ErrorMessage = "Hours worked must be between 0.5 and 160")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Display(Name = "Hourly Rate")]
        [Range(10, 500, ErrorMessage = "Hourly rate must be between 10 and 500")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount => HoursWorked * HourlyRate;

        [Display(Name = "Additional Notes")]
        [StringLength(500)]
        public string AdditionalNotes { get; set; } // REMOVED [Required]

        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [Display(Name = "Rejection Notes")]
        [StringLength(1000)]
        public string RejectionNotes { get; set; }

        [Display(Name = "Rejected By")]
        public string RejectedBy { get; set; }

        [Display(Name = "Verified By")]
        public string VerifiedBy { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        public virtual ICollection<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
    }

    public class SupportingDocument
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
    }
}