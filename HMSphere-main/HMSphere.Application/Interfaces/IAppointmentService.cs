using HMSphere.Application.DTOs;
using HMSphere.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Interfaces
{
	public interface IAppointmentService
	{
        Task<List<AppointmentDto>> GetAllAppointmentsByPatientIdAsync(string patientId);
        Task<AppointmentDto> GetAppointmentByIdAsync(int id);

        Task<AppointmentDto> CreateAppointment(AppointmentDto appointmentDto);
        Task<AppointmentDto> CreateAppointmentByAdmin(AppointmentDto appointmentDto);
        Task<AppointmentDto> UpdateAppointment(AppointmentDto appointmentDto);
        Task<bool> ApproveAppointment(int appointmentId, bool isApproved);
        Task<bool> CancelAppointment(int appointmentId);
        Task<IEnumerable<AppointmentDto>> GetPendingAppointments();

        Task<List<AppointmentDto>> SearchByStatus(string status, string doctorId);
    }
}
