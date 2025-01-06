using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Interfaces
{
    public interface IShiftService
    {
        Task<bool> AssignStaffToShiftAsync(int shiftId, string staffId);
        Task<bool> AssignDoctorToShiftAsync(int shiftId, string doctorId);
    }
}
