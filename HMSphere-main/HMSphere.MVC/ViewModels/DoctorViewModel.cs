using HMSphere.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HMSphere.MVC.ViewModels
{
	public class DoctorViewModel
	{
		public string Id { get; set; }
		public string FirstName { get; set; } 
		public string LastName { get; set; } 
		public string PhoneNumber { get; set; }
		public string Specialization { get; set; }
        public string? DepartmentName { get; set; }
        public int DepartmentId { get; set; }
		public List<SelectListItem>? Departments { get; set; }
		public int UpcomingAppointmentsCount { get; set; }
		public int NumberOfPatients { get; set; }
		public int NumberOfMedicalRecords { get; set; }

		public List<MedicalRecordViewModel>? LatestMedicalRecords { get; set; }
		public List<AppointmentsViewModel>? LatestAppointments { get; set; }
	}
}
