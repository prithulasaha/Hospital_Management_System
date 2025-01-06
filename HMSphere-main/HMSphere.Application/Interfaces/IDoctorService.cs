using HMSphere.Application.DTOs;
using HMSphere.Domain.Entities;
using HMSphere.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<List<DoctorDto>> GetAll();
        public Task<List<Doctor>> GetDoctorsByDepartmentIdAsync(int? departmentId);
        Task<List<PatientDto>> GetAllPatientAsync(string doctorId);
        Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync(string pateintId);
		Task<bool> AddMedicalRecordAsync(MedicalRecordDto entity, string doctorId , string patientId);
        Task<ResponseDTO> Profile(string doctorId);
        Task<int> GetNext7DaysAppointments(string doctorId);
        Task<int> GetNumberOfPatients(string doctorId);
        Task<int> GetNumberOfMedicalRecords(string doctorId);
        Task<List<AppointmentDto>> GetLatestAppointments(string doctorIddoctorI);
        Task<List<MedicalRecordDto>> GetLatestMedicalRecords(string doctorId);
        Task<List<AppointmentDto>> GetAllAppointments(string doctorId);
        Task<ResponseDTO> GetAppointmentDetails(int appointmentId);
        Task<ResponseDTO> GetMedicalRecordDetails(int recordId);
	}
}
