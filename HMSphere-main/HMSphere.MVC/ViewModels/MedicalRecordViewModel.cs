using System.ComponentModel.DataAnnotations;

namespace HMSphere.MVC.ViewModels
{
    public class MedicalRecordViewModel
    {
        public int? Id { get; set; }
        public string? DoctorId { get; set; }
		public string? PatientId { get; set; }

		[Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string? Diagnosis { get; set; }

        [Required]
        public string? TreatmentPlan { get; set; }
        [Required]
        public string? Medications { get; set; }

        [Required]
        public string? DoctorNotes { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }

        public string? PatientFirstName { get; set; }
        public string? PatientLastName { get; set; }
        public string? DoctorFirstName { get; set; }
        public string? DoctorLastName { get; set; }

        public string? PatientName => PatientFirstName + " " + PatientLastName;
        public string? DoctorName => DoctorFirstName + " " + DoctorLastName;
    }
}
