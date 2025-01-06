namespace HMSphere.Application.DTOs
{
    public class PatientDto
	{
		public string PatientId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public double Weight { get; set; }
		public double Height { get; set; }
		public string PhoneNumber { get; set; }
		public string Blood { get; set; }
		public string NID { get; set; }
		public string Gender { get; set; }
		public int Age { get; set; }
		public DateTime? LastVisitDate { get; set; }


	}
}
