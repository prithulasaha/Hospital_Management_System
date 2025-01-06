using HMSphere.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace HMSphere.MVC.ViewModels
{
	public class PatientsHistoryViewModel
	{
        public  string PatientId { get; set; }
        [Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public double Weight { get; set; }

		[Required]
		public double Height { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public string Blood { get; set; }

		[Required]
		public string NID { get; set; }

		[Required]
		public string Gender { get; set; }
		public NextAppointmentViewModel? NextAppointment { get; set; }
        public List<MedicalRecordViewModel>? LatestMedicalRecords { get; set; }
        public List<AppointmentViewModel>? LatestAppointments { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }

		public int CalculateAgeFromNID(string nationalId)
		{
			if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length < 6)
				return 0;
			string dobString = nationalId.Substring(0, 6);

			int year = int.Parse(dobString.Substring(0, 2));
			int month = int.Parse(dobString.Substring(2, 2));
			int day = int.Parse(dobString.Substring(4, 2));

			year += (year <= DateTime.Now.Year % 100) ? 2000 : 1900;

			var birthDate = new DateTime(year, month, day);
			var age = DateTime.Now.Year - birthDate.Year;

			if (DateTime.Now < birthDate.AddYears(age))
			{
				age--;
			}
			return age;
		}
		public DateTime? GetLastVisitDate()
		{
			// Return the most recent appointment date if available, otherwise null
			return Appointments?.OrderByDescending(a => a.Date).FirstOrDefault()?.Date;
		}

		public int Age => CalculateAgeFromNID(NID);

    }
}
