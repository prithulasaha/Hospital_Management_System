using HMSphere.Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMSphere.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HMSphere.Domain.Entities;
using System.Reflection.Metadata.Ecma335;
using HMSphere.Application.Interfaces;

namespace HMSphere.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly HmsContext _context;
        private readonly IMapper _mapper;

        public PatientService(HmsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PatientDto>> GetAll()
        {
            try
            {
                var patients = await _context.Patients.Include(p => p.User)
                    .ToListAsync();
                if (!patients.Any())
                {
                    return new List<PatientDto>();
                }
                var dto = patients.Select(p => _mapper.Map<PatientDto>(p)).ToList();
                return dto;
            }
            catch
            {
                return new List<PatientDto>();
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync(string patientId)
        {
            var appointments = await _context.Appointments
               .Where(a => a.PatientId == patientId)
               .OrderByDescending(a => a.Date)
               .ToListAsync();
            return appointments.Select(a => _mapper.Map<AppointmentDto>(a)).ToList();
        }
        public async Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync(string patientId)
        {
            var medicalrecords = await _context.MedicalRecords
              .Where(mr => mr.PatientId == patientId)
              .OrderByDescending(mr => mr.CreatedDate)
              .ToListAsync();
            return medicalrecords.Select(mr => _mapper.Map<MedicalRecordDto>(mr)).ToList();
        }
        public async Task<IEnumerable<AppointmentDto>> GetLast5AppointmentsAsync(string patientId)
        {
            var Latest5Appointments = await _context.Appointments
                                 .Include(a=>a.Doctor)
                                 .ThenInclude(d=>d.User)
                                 .Where(a => a.PatientId == patientId)
                                 .OrderByDescending(a => a.Date)
                                 .Take(5)
                                 .ToListAsync();
            if (Latest5Appointments.Count()<=0)
            {
                return new List<AppointmentDto>();
            }
            return Latest5Appointments.Select(a => _mapper.Map<AppointmentDto>(a)).ToList();
        }
        public async Task<IEnumerable<MedicalRecordDto>> GetLast5MedicalRecordsAsync(string patientId)
        {
            var Latest5MedicalRecords = await _context.MedicalRecords
                                 .Where(mr => mr.PatientId == patientId)
                                 .OrderByDescending(mr => mr.CreatedDate)
                                 .Take(5)
                                 .ToListAsync();
            if (Latest5MedicalRecords.Count()<=0)
            {
                return new List<MedicalRecordDto>();
            }
            return Latest5MedicalRecords.Select(mr => _mapper.Map<MedicalRecordDto>(mr)).ToList();
        }

        public async Task<ResponseDTO> Profile(string id)
        {
            try
            {
                var patient = await _context.Patients.Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.Id == id);
                if (patient != null)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Model = patient
                    };
                }

                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "patient not found!"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { IsSuccess = false, Message = "An error occured, please try again", StatusCode = 500 };
            }
        }
        public async Task<NextAppointmentDto> GetNextAppointmentByPatientIdAsync(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
                throw new ArgumentException("Patient ID cannot be null or empty.");

            var currentDate = DateTime.Now;

            var nextAppointment = await _context.Appointments
                .Where(a => a.PatientId == patientId && a.Date >= currentDate)
                .OrderBy(a => a.Date)
                .Select(a => new NextAppointmentDto
                {
                    DoctorName = a.Doctor.User.UserName,
                    AppointmentDate = a.Date

                    //AppointmentDate = a.Date.ToString("dddd, dd MMMM")
                })
                .FirstOrDefaultAsync();

            return nextAppointment;
        }

        public async Task<List<Patient>> GetPatients()
        {
           
                return await _context.Patients.Include(d => d.User).ToListAsync();
          
        }

        public async Task<MedicalRecordDto> GetMedicalRecordAsync(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                                      .Include(a => a.Patient)
                                          .ThenInclude(p => p.User)
                                      .Include(a => a.Doctor)
                                          .ThenInclude(d => d.User)
                                      .FirstOrDefaultAsync(a => a.Id == id);

            if (medicalRecord == null)
            {
                return null;
            }

            var medicalRecordDto = new MedicalRecordDto
            {
               Id= medicalRecord.Id,
               PatientId = medicalRecord.PatientId,
               DoctorId = medicalRecord.DoctorId,
               DoctorNotes = medicalRecord.DoctorNotes,
               CreatedDate=medicalRecord.CreatedDate,
               Diagnosis=medicalRecord.Diagnosis,
               TreatmentPlan=medicalRecord.TreatmentPlan,
               Medications=medicalRecord.Medications,   


            };

            return medicalRecordDto;
        }

        public async Task<List<PatientDto>> SearchByName(string name,string doctorId)
        {
            var patients = await _context.MedicalRecords.Include(p => p.Patient.User)
                .Where(m => m.Patient.User.FirstName.Contains(name)
                && m.DoctorId==doctorId).Select(m=>m.Patient).Distinct()
                .ToListAsync();
            if (patients.Any())
            {
                return patients.Select(p => _mapper.Map<PatientDto>(p)).ToList();
            }
            return new List<PatientDto>();
        }

        public async Task<List<PatientDto>> SearchByNID(string nid, string doctorId)
        {
            var patients = await _context.MedicalRecords.Include(p => p.Patient.User)
                .Where(m => m.Patient.User.NID.Contains(nid)
                && m.DoctorId == doctorId).Select(m => m.Patient).Distinct()
                .ToListAsync();
            if (patients.Any())
            {
                return patients.Select(p => _mapper.Map<PatientDto>(p)).ToList();
            }
            return new List<PatientDto>();
        }

		public async Task<List<PatientDto>> SearchByBloodType(string type, string doctorId)
		{
			var patients = await _context.MedicalRecords.Include(p => p.Patient.User)
				.Where(m => m.Patient.Blood.Contains(type)
				&& m.DoctorId == doctorId).Select(m => m.Patient).Distinct()
				.ToListAsync();
			if (patients.Any())
			{
				return patients.Select(p => _mapper.Map<PatientDto>(p)).ToList();
			}
			return new List<PatientDto>();
		}
	}
}
