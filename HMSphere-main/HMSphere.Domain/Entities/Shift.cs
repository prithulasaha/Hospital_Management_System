using HMSphere.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Domain.Entities
{
    public class Shift
    {
        public int Id { get; set; }
        public ShiftType Type { get; set; }= ShiftType.Morning;
        public TimeOnly StartTime { get; set; }=TimeOnly.FromDateTime(DateTime.Now);
        public TimeOnly EndTime { get; set; }= TimeOnly.FromDateTime(DateTime.Now);
        public string? Notes { get; set; }= string.Empty;
        public bool IsActive { get; set; }=false;
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<DoctorShift> DoctorShifts { get; set; }=new List<DoctorShift>();
        public virtual ICollection<StaffShift> StaffShifts { get; set; }= new List<StaffShift>();
    }
}