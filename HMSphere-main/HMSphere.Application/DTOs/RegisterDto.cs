using HMSphere.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.DTOs
{
    public class RegisterDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; } 
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? NID { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Role {  get; set; }
        public string? PhoneNumber {  get; set; }

		//for doctor
		public string? Specialization { get; set; }
        public int? DepartmentId { get; set; }

        //for patient
        public string? Blood { get; set; }
		public string? DiseaseHistory { get; set; }
		public double? Weight { get; set; }
		public double? Height { get; set; }
        //for staff
		public string? JobTitle { get; set; } 
	}
}
