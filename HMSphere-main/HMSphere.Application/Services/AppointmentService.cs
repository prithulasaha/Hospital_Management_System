using AutoMapper;
using HMSphere.Application.DTOs;
using HMSphere.Application.Interfaces;
using HMSphere.Application.Mailing;
using HMSphere.Domain.Entities;
using HMSphere.Domain.Enums;
using HMSphere.Infrastructure.DataContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HmsContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailingService _emailService;

        public AppointmentService(HmsContext context, 
                                  UserManager<ApplicationUser> userManager,
                                  IHttpContextAccessor httpContextAccessor,
                                  IMapper mapper,
                                  IMailingService emailService
                                   )
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _emailService = emailService;
        }
        public async Task<List<AppointmentDto>> GetAllAppointmentsByPatientIdAsync(string patientId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d=>d.User)
                .ToListAsync();

            var appointmentDtos = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Date = a.Date,
                Status = a.Status,
                ReasonFor = a.ReasonFor ?? string.Empty,  // Check for null
                AppointmentTime = a.AppointmentTime,
                IsApproved = a.IsApproved ?? false,           // Null-safe access for bool?
                DoctorName = a.Doctor.User.UserName
            }).ToList();

               return appointmentDtos;
        }
        public async Task<AppointmentDto> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
              return null; 
            }

            var appointmentDto = new AppointmentDto
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Status = appointment.Status,
                ReasonFor = appointment.ReasonFor ?? string.Empty,  // Check for null
                PatientId = appointment.PatientId,
                AppointmentTime = appointment.AppointmentTime,
                DoctorId = appointment.DoctorId,
                PatientName = appointment.Patient?.User?.UserName ?? "Unknown",   // Null-safe access
                DoctorName = appointment.Doctor?.User?.UserName ?? "Unknown",     // Null-safe access
                IsApproved = appointment.IsApproved ?? false,           // Null-safe access for bool?
            };

            return appointmentDto;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return new List<Appointment> { new Appointment() };
        }

        public async Task<AppointmentDto> CreateAppointment(AppointmentDto appointmentDto)
        {
            if (appointmentDto.Date == null || appointmentDto.AppointmentTime == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid date or time for the appointment."
                };
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == appointmentDto.DoctorId);
            if (doctor == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected doctor does not exist."
                };
            }

            var department = await _context.Departments.FindAsync(appointmentDto.DepartmentId);
            if (department == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected department is invalid."
                };
            }

            // Check for conflicting appointments
            var conflictingAppointment = await _context.Appointments
                .AnyAsync(a => a.DoctorId == appointmentDto.DoctorId
                               && a.Date == appointmentDto.Date
                               && a.AppointmentTime == appointmentDto.AppointmentTime);

            if (conflictingAppointment)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected doctor is not available at this time."
                };
            }

            var appointment = _mapper.Map<Appointment>(appointmentDto);

            appointment.PatientId = await GetCurrentUserAsync();

            appointment.DoctorId = doctor.Id;
            appointment.Doctor = doctor;

            var patient = await _context.Patients.FindAsync(appointment.PatientId);
            if (patient == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Patient not found."
                };
            }
            appointment.Patient = patient;
            appointment.Status = Status.Pending;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            appointmentDto.Status = Status.Pending;
            appointmentDto.IsApproved = null;
            appointmentDto.Id = appointment.Id;
            appointmentDto.PatientId = appointment.PatientId;
            appointmentDto.IsSuccessful = true;
            appointmentDto.ErrorMessage = null;

            return appointmentDto;
        }
        public async Task<AppointmentDto> CreateAppointmentByAdmin(AppointmentDto appointmentDto)
        {
            if (appointmentDto.Date == null || appointmentDto.AppointmentTime == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid date or time for the appointment."
                };
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == appointmentDto.DoctorId);
            if (doctor == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected doctor does not exist."
                };
            }

            var patient = await _context.Patients.FindAsync(appointmentDto.PatientId);
            if (patient == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected Patient is invalid."
                };
            }

            // Check for conflicting appointments
            var conflictingAppointment = await _context.Appointments
                .AnyAsync(a => a.DoctorId == appointmentDto.DoctorId
                               && a.Date == appointmentDto.Date
                               && a.AppointmentTime == appointmentDto.AppointmentTime);

            if (conflictingAppointment)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected doctor is not available at this time."
                };
            }

            var appointment = _mapper.Map<Appointment>(appointmentDto);

            

            appointment.DoctorId = doctor.Id;
            appointment.Doctor = doctor;

         
            if (patient == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Patient not found."
                };
            }
            
            appointment.Patient = patient;
            appointment.PatientId = patient.Id;
            appointment.Status = Status.Pending;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            appointmentDto.Status = Status.Pending;
            appointmentDto.IsApproved = null;
            appointmentDto.Id = appointment.Id;
            appointmentDto.PatientId = appointment.PatientId;
            appointmentDto.IsSuccessful = true;
            appointmentDto.ErrorMessage = null;

            return appointmentDto;
        }

        public async Task<AppointmentDto> UpdateAppointment(AppointmentDto appointmentDto)
        {
            if (appointmentDto.Id == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Appointment ID is required for update."
                };
            }

            var existingAppointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentDto.Id);

            if (existingAppointment == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Appointment not found."
                };
            }

            if (appointmentDto.Date == null || appointmentDto.AppointmentTime == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid date or time for the appointment."
                };
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == appointmentDto.DoctorId);
            if (doctor == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected doctor does not exist."
                };
            }

            var department = await _context.Departments.FindAsync(appointmentDto.DepartmentId);
            if (department == null)
            {
                return new AppointmentDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "The selected department is invalid."
                };
            }

            if (existingAppointment.DoctorId != appointmentDto.DoctorId || existingAppointment.Date != appointmentDto.Date || existingAppointment.AppointmentTime != appointmentDto.AppointmentTime)
            {
                var conflictingAppointment = await _context.Appointments
                    .AnyAsync(a => a.DoctorId == appointmentDto.DoctorId
                                   && a.Date == appointmentDto.Date
                                   && a.AppointmentTime == appointmentDto.AppointmentTime
                                   && a.Id != appointmentDto.Id); 

                if (conflictingAppointment)
                {
                    return new AppointmentDto
                    {
                        IsSuccessful = false,
                        ErrorMessage = "The selected doctor is not available at this time."
                    };
                }
            }

            // Map updated details
            existingAppointment.DoctorId = doctor.Id;
            existingAppointment.Date = appointmentDto.Date;
            existingAppointment.AppointmentTime = appointmentDto.AppointmentTime;
            existingAppointment.ReasonFor = appointmentDto.ReasonFor;

            await _context.SaveChangesAsync();

            appointmentDto.IsSuccessful = true;
            appointmentDto.ErrorMessage = null;

            return appointmentDto;
        }

        public async Task<bool> ApproveAppointment(int appointmentId, bool isApproved)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)  // Ensure User is included with Patient
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return false; // Appointment not found
            }

            appointment.IsApproved = isApproved;
            appointment.Status = isApproved ? Status.Completed : Status.Cancelled;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            // Check if Patient and User are not null before sending the email
            if (appointment.Patient != null && appointment.Patient.User != null)
            {
                string subject = isApproved ? "Appointment Approved" : "Appointment Rejected";
                string message = isApproved
                    ? $"Dear {appointment.Patient.User.UserName}, your appointment on {appointment.Date} has been approved."
                    : $"Dear {appointment.Patient.User.UserName}, your appointment on {appointment.Date} has been rejected.";

                if (!string.IsNullOrEmpty(appointment.Patient.User.Email))
                {
                    await _emailService.SendMailAsync(appointment.Patient.User, subject, message);
                }
                else
                {
                    string adminMessage = $"The email for user {appointment.Patient.User.UserName} (Appointment ID: {appointment.Id}) is missing.";
                    await _emailService.SendMailAsync(appointment.Patient.User, "Missing Email for Appointment", adminMessage);
                }
            }
            else
            {
                return false;
            }

            return true;
        }


        public async Task<IEnumerable<AppointmentDto>> GetPendingAppointments()
        {
            var pendingAppointments = await _context.Appointments
                .Where(a => a.Status == Status.Pending)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User) 
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User) 
                .ToListAsync();

            return pendingAppointments.Select(appointment => new AppointmentDto
            {
                Id = appointment.Id,
                Date = appointment.Date,
                DoctorName = appointment.Doctor != null && appointment.Doctor.User != null
                    ? appointment.Doctor.User.UserName
                    : "Unknown Doctor",
                PatientName = appointment.Patient != null && appointment.Patient.User != null
                    ? appointment.Patient.User.UserName
                    : "Unknown Patient",
                Status = appointment.Status,
                IsApproved = appointment.IsApproved,
            }).ToList();
        }


        private async Task<string> GetCurrentUserAsync()
        {
            var userClaim = _httpContextAccessor.HttpContext!.User;

            // Get the current user from claims
            var user = await _userManager.GetUserAsync(userClaim);

            return user.Id;
        }

        public async Task<bool> CancelAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return false;
            }

            if (appointment.Status !=Status.Pending)
            {
                return false;
            }

            appointment.Status = HMSphere.Domain.Enums.Status.Cancelled;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<AppointmentDto>> SearchByStatus(string status, string doctorId)
        {
			var appointments=new List<Appointment>();

			if (Enum.TryParse<Status>(status, true, out var parsedStatus))
            {
                appointments = await _context.Appointments.Include(a => a.Doctor.User)
                    .Include(a => a.Patient.User).Where(a => a.Status == parsedStatus && a.DoctorId == doctorId)
                    .ToListAsync();
            }
            if (appointments.Any())
            {
                return appointments.Select(a=>_mapper.Map<AppointmentDto>(a)).ToList();

            }
            return new List<AppointmentDto>();
        }

    }

}
