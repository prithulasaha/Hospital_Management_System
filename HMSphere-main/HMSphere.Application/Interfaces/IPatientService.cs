using HMSphere.Application.DTOs;
using HMSphere.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync(string PateintID);
        Task<IEnumerable<AppointmentDto>> GetLast5AppointmentsAsync(string PatientID);
        Task<IEnumerable<MedicalRecordDto>> GetLast5MedicalRecordsAsync(string PatientID);
        Task<ResponseDTO> Profile(string Id);
        Task<NextAppointmentDto> GetNextAppointmentByPatientIdAsync(string patientId);
        Task<MedicalRecordDto> GetMedicalRecordAsync(int id);
        Task<List<PatientDto>> GetAll();
        Task<List<Patient>> GetPatients();
        Task<List<PatientDto>> SearchByName(string name, string doctorId);
        Task<List<PatientDto>> SearchByNID(string nid, string doctorId);
        Task<List<PatientDto>> SearchByBloodType(string type, string doctorId);
    }
}
