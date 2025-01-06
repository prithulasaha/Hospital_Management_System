using HMSphere.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;


namespace HMSphere.Domain.Entities
{
    public class Staff
    {
        public string Id {  get; set; } // will be same as AppUser Id
		public Role Role { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public DateOnly? HireDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int? DepartmentId { get; set; }
        public virtual ApplicationUser User { get; set; }
		[ForeignKey("DepartmentId")]
		public virtual Department? Department { get; set; }
        public virtual ICollection<StaffShift> StaffShifts { get; set; } = new List<StaffShift>();

    }
}
