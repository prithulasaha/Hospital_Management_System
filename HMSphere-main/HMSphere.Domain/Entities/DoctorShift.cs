using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Domain.Entities
{
    public class DoctorShift
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string DoctorId { get; set; }
        public int ShiftId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Shift Shift { get; set; }

    }
}
