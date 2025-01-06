using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.DTOs
{
	public class StaffDto
	{
        public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
        public string DepartmentName { get; set; }
		public string JobTitle { get; set; }
        public DateOnly? HireDate { get; set; }
        public string StaffShift { get; set; }
		public string PhoneNumber { get; set; }
	}
}
