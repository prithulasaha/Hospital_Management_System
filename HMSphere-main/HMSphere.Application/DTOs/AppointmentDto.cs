using HMSphere.Domain.Entities;
using HMSphere.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.DTOs
{
	public class AppointmentDto
	{
        public int? Id { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public Status? Status { get; set; } = Domain.Enums.Status.Pending;
        public string? ReasonFor { get; set; } = string.Empty;
        public string? Clinic { get; set; } = string.Empty;
        public string? PatientId { get; set; }
        public int? DepartmentId { get; set; }
        public TimeSpan? AppointmentTime { get; set; }
        public string? DoctorId { get; set; }
        public bool? IsApproved { get; set; }=false;
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? ErrorMessage { get; set; }  
        public bool IsSuccessful { get; set; } = false; 
        

       

	}
}

