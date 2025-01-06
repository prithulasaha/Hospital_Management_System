using HMSphere.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HMSphere.MVC.ViewModels
{
	public class AppointmentsViewModel
	{
        public int Id { get; set; }
        [Required]
		public DateTime Date { get; set; }
		[Required]
		public Status Status { get; set; }
		[Required]
		public string ReasonFor { get; set; }
		[Required]
		public string Clinic { get; set; }
		[Required]
        public TimeSpan? AppointmentTime { get; set; }
        [Required]
		public bool IsDeleted { get; set; }
		[Required]
		public string DoctorID { get; set; }
		[Required]
		public string PatientID { get; set; }
		public string PatientName { get; set; }
	}
}
