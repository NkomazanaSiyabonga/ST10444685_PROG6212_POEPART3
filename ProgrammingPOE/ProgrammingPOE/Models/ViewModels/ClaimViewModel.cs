using System.ComponentModel.DataAnnotations;

namespace ProgrammingPOE.ViewModels
{
    public class ClaimViewModel
    {
        [Required(ErrorMessage = "Lecturer name is required")]
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Display(Name = "Hours Worked")]
        [Range(0.5, 160, ErrorMessage = "Hours worked must be between 0.5 and 160")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Display(Name = "Hourly Rate")]
        [Range(10, 850, ErrorMessage = "Hourly rate must be between 10 and 850")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Additional Notes")]
        [StringLength(500, ErrorMessage = "Additional notes cannot exceed 500 characters")]
        public string AdditionalNotes { get; set; } // NO [Required] attribute

        [Display(Name = "Supporting Documents")]
        public List<IFormFile> SupportingDocuments { get; set; } = new List<IFormFile>();
    }
}