using HMSphere.Application.DTOs;
using HMSphere.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Interfaces
{
	public interface IStaffService
	{
        //Task<IEnumerable<object>> GetAllAsync();
        Task<IEnumerable<ShiftDto>> GetShiftsForStaffAsync(string StaffId);
		Task<IEnumerable<StaffDto>> GetAllAsync();
        Task<ResponseDTO> Profile(string Id);


    }
}
