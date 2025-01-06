using HMSphere.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HMSphere.MVC.ViewModels
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "First name is required")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "NID is required")]
		[RegularExpression(@"^\d{14}$", ErrorMessage = "NID must be a 14-digit number")]
		public string NID { get; set; }

		[Required(ErrorMessage = "Phone number is required")]
		[Phone(ErrorMessage = "Invalid phone number")]
		[RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be a 11-digit number")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		public string Gender { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "The Role is required")]
		public string Role { get; set; }

		// Conditional fields (doctor, patient, staff)
		public string? Specialization { get; set; }
		//public int? DepartmentId { get; set; }
		public string Username { get; set; }
		public string? Blood { get; set; }
		public string? DiseaseHistory { get; set; }
		public double? Weight { get; set; }
		public double? Height { get; set; }
		public string? JobTitle { get; set; }
		public int? DepartmentId { get; set; }
		public IEnumerable<SelectListItem>? Departments { get; set; }
		public DateTime? HireDate { get; set; } = DateTime.Now;

	}
}
