using HMSphere.Domain.Entities;
using HMSphere.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMSphere.MVC.ViewModels
{
    public class StaffViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
		public string LastName { get; set; }
		public string JobTitle { get; set; }
		public string PhoneNumber { get; set; }
		public DateOnly? HireDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
		public string? DepartmentName { get; set; }
		public int DepartmentId { get; set; }
		public List<SelectListItem>? Departments { get; set; }
	}
}
