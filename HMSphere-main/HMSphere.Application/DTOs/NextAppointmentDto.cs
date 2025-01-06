using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.DTOs
{
    public class NextAppointmentDto
    {
        public string? DoctorName { get; set; }
        public DateTime? AppointmentDate { get; set; }
    }
}
