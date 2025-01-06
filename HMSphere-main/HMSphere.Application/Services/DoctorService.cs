using AutoMapper;
using HMSphere.Application.DTOs;
using HMSphere.Application.Interfaces;
using HMSphere.Domain.Entities;
using HMSphere.Domain.Enums;
using HMSphere.Infrastructure.DataContext;
using HMSphere.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Services
{
	public class DoctorService : IDoctorService
	{
		private readonly HmsContext _context;
		private readonly IMapper _mapper;
        //private readonly IBaseRepository<MedicalRecord> _medicalRecord;
        private readonly IBaseRepository<Doctor> _doctorRepo;

        public DoctorService(HmsContext context, IMapper mapper, IBaseRepository<Doctor> doctorRepo)
		{
			_context = context;
			_mapper = mapper;
			_doctorRepo = doctorRepo;
		}

        public async Task<List<DoctorDto>> GetAll()
		{
			try
			{
				//var doctors = await _doctorRepo.GetAllAsync();
				var doctors=await _context.Doctors.Include(d=>d.User)
					.Include(d=>d.Department).ToListAsync();
				if (!doctors.Any())
				{
					return new List<DoctorDto>();
				}

				var dto = doctors.Select(d => _mapper.Map<DoctorDto>(d)).ToList();
				return dto;
			}
			catch
			{
                return new List<DoctorDto>();

            }
        }


        public async Task<ResponseDTO> Profile(string id)
		{
			try
			{
				var doctor = await _context.Doctors.Include(d=>d.User)
					.Include(d=>d.Department)
					.FirstOrDefaultAsync(d=>d.Id==id);
				if (doctor != null)
				{
					return new ResponseDTO
					{
						IsSuccess = true,
						StatusCode = 200,
						Model = doctor
					};
				}

                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message="Doctor not found!"
                };
            }
			catch (Exception ex)
			{
				return new ResponseDTO { IsSuccess = false, Message = "An error occured, please try again", StatusCode = 500 };
			}
		}

        public async Task<List<PatientDto>> GetAllPatientAsync(string doctorId)
		{
			var patients = await _context.MedicalRecords
				.Include(m=>m.Patient.User)
				.Where(m => m.DoctorId == doctorId && !m.IsDeleted)
				.Select(m => m.Patient)
				.Distinct()
				.ToListAsync();
			var patientsResult = patients.Select(p => _mapper.Map<PatientDto>(p)).ToList();
			return patientsResult;
		}
		public async Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync(string patientId)
		{
			var medicalRecords = await _context.MedicalRecords
				.Where(di => di.PatientId == patientId)
				.ToListAsync();
			var medicalRecordsResult = medicalRecords.Select(mr => _mapper.Map<MedicalRecordDto>(mr)).ToList();
			return medicalRecordsResult;
		}
		public async Task<bool> AddMedicalRecordAsync(MedicalRecordDto entity, string doctorId , string patientId)
		{ 
			return true;
		}

		public async Task<int> GetNext7DaysAppointments(string id)
		{
			try
			{
				var appointments = await _context.Appointments.Where(a => a.DoctorId == id
							&& a.Date <= DateTime.Now.AddDays(7)).ToListAsync();
				if (!appointments.Any())
				{
					return 0;
				}
				var appoints = appointments.Count();
				return appoints;
			}
			catch (Exception ex)
			{
				return 0;
			}
		}

		

        public async Task<int> GetNumberOfPatients(string id)
		{
			var patients = await _context.MedicalRecords.Where(m => m.DoctorId == id).ToListAsync();
			if (!patients.Any())
			{
				return 0;
			}

			var numOfPatients=patients.Count();
			return numOfPatients;
		}
		
		public async Task<int> GetNumberOfMedicalRecords(string id)
		{
			var records = await _context.MedicalRecords.Where(m => m.DoctorId == id
			&& m.LastUpdated >= DateTime.Now.AddDays(-7) && m.LastUpdated <= DateTime.Now).ToListAsync();
			if (!records.Any())
			{
				return 0;
			}
			var numberOfMedicalRecords=records.Count();
			return numberOfMedicalRecords;
        }

		public async Task<List<AppointmentDto>> GetLatestAppointments(string id)
		{
			var appointments = await _context.Appointments
				.Include(a=>a.Patient.User)
				.Where(a => a.DoctorId == id).OrderByDescending(a => a.Date).Take(7)
			.ToListAsync();

			if (!appointments.Any())
			{
				return new List<AppointmentDto>();
			}

			List<AppointmentDto> appointmentDtos = new();
			foreach (var appointment in appointments)
			{
				var dto=_mapper.Map<AppointmentDto>(appointment);
				appointmentDtos.Add(dto);
			}
			return appointmentDtos;
		}

		public async Task<List<MedicalRecordDto>> GetLatestMedicalRecords(string id)
		{
			var records = await _context.MedicalRecords.Where(m => m.DoctorId == id)
				.OrderByDescending(m => m.LastUpdated).Take(7).ToListAsync();
			if (!records.Any())
			{
				return new List<MedicalRecordDto>();
			}

			List<MedicalRecordDto> recordDtos = new();
			foreach (var record in records)
			{
				var dto=_mapper.Map<MedicalRecordDto>(record);
				recordDtos.Add(dto);
			}
			return recordDtos;
		}

		public async Task<List<AppointmentDto>> GetAllAppointments(string doctorId)
		{
			var appointments = await _context.Appointments
				.Include(a => a.Patient.User)
				.Where(a => a.DoctorId == doctorId
			&& !a.IsDeleted).ToListAsync();
			if (!appointments.Any())
			{
				return new List<AppointmentDto>();
			}
			var appointmentDtos = appointments.Select(a => _mapper.Map<AppointmentDto>(a)).ToList();
			return appointmentDtos;
		}

		public async Task<ResponseDTO> GetAppointmentDetails(int appointmentId)
		{
			try
			{
				var appointment = await _context.Appointments
					.Include(a=>a.Patient.User).FirstOrDefaultAsync(a=>a.Id==appointmentId);
				if (appointment == null)
				{
					return new ResponseDTO
					{
						IsSuccess = false,
						StatusCode = 404,
						Message = "Not found"
					};
				}

				var dto = _mapper.Map<AppointmentDto>(appointment);
				return new ResponseDTO
				{
					IsSuccess = true,
					StatusCode = 200,
					Model = dto
				};
			}
			catch(Exception ex)
			{
				return new ResponseDTO
				{
					IsSuccess = false,
					StatusCode = 500,
					Message = ex.Message
				};
			}
		}

        public async Task<List<Doctor>> GetDoctorsByDepartmentIdAsync(int? departmentId)
        {
            if (departmentId == null)
            {
                return await _context.Doctors.Include(d => d.User).ToListAsync();
            }

            var doctors = await _context.Doctors
                .Where(d => d.DepartmentId == departmentId)
                .Include(d => d.User)
                .ToListAsync();

            return doctors;
        }

        public async Task<ResponseDTO> GetMedicalRecordDetails(int recordId)
		{
			try
			{
				var record = await _context.MedicalRecords.Include(m => m.Patient.User)
					.Include(m => m.Doctor.User).FirstOrDefaultAsync(m => m.Id == recordId);
				if(record != null)
				{
					var dto = _mapper.Map<MedicalRecordDto>(record);
                    return new ResponseDTO
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Model = dto
                    };
                }
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Not found"
                };
            }
			catch (Exception ex)
			{
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
		}


    }
}
