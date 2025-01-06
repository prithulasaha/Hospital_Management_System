using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Domain.Entities
{
    public class Doctor
    {
        public string Id { get; set; } // will be same as AppUser Id
        public string? Specialization { get; set; } 
        public int? DepartmentId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public virtual ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<DoctorShift> DoctorShifts { get; set; } = new List<DoctorShift>();
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; } = new List<MedicalRecord>();

    }
}
