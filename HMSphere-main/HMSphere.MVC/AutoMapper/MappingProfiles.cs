using AutoMapper;
using HMSphere.Application.DTOs;
using HMSphere.Domain.Entities;
using HMSphere.MVC.ViewModels;

namespace HMSphere.MVC.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AuthDto, AuthViewModel>().ReverseMap();
            CreateMap<RegisterDto, RegisterViewModel>().ReverseMap();
            CreateMap<LoginDto, LoginViewModel>().ReverseMap();
            CreateMap<MedicalRecordDto, PatientMedicalRecordsViewModel>().ReverseMap();
            CreateMap<AppointmentDto, PatientAppointmentsViewModel>().ReverseMap();
			CreateMap<ShiftViewModel, ShiftDto>().ReverseMap();
			CreateMap<ShiftDto, Shift>().ReverseMap();
            CreateMap<NextAppointmentDto, NextAppointmentViewModel>().ReverseMap();

            CreateMap<PatientDto , PatientsHistoryViewModel>().ReverseMap();
            {
                CreateMap<Patient, PatientDto>()
                    .ForMember(dest => dest.PatientId, p => p.MapFrom(src => src.Id))
                    .ForMember(dest => dest.FirstName, p => p.MapFrom(src => src.User.FirstName))
                    .ForMember(dest => dest.LastName, p => p.MapFrom(src => src.User.LastName))
                    .ForMember(dest => dest.Gender, p => p.MapFrom(src => src.User.Gender))
                    .ForMember(dest => dest.PhoneNumber, p => p.MapFrom(src => src.User.PhoneNumber))
                    .ReverseMap();
            }

            CreateMap<DoctorDto , DoctorViewModel>().ReverseMap();
            {
                CreateMap<Doctor, DoctorDto>()
                    .ForMember(dest => dest.FirstName, d => d.MapFrom(d => d.User.FirstName))
                    .ForMember(dest => dest.LastName, d => d.MapFrom(d => d.User.LastName))
                    .ForMember(dest => dest.PhoneNumber, d => d.MapFrom(d => d.User.PhoneNumber))
                    .ForMember(dest => dest.DepartmentName, d => d.MapFrom(d => d.Department.Name))
                    .ReverseMap();
            }

            CreateMap<StaffDto , StaffViewModel>().ReverseMap();
            {
                CreateMap<Staff, StaffDto>()
                    .ForMember(dest => dest.FirstName, s => s.MapFrom(src => src.User.FirstName))
                    .ForMember(dest => dest.LastName, s => s.MapFrom(src => src.User.LastName))
                    .ForMember(dest => dest.PhoneNumber, s => s.MapFrom(src => src.User.PhoneNumber))
                    .ForMember(dest => dest.DepartmentName, s => s.MapFrom(src => src.Department.Name))
                    .ReverseMap();
            }

			CreateMap<MedicalRecordDto , MedicalRecordViewModel>().ReverseMap();
            {
                CreateMap<MedicalRecord, MedicalRecordDto>()
                    .ForMember(dest=>dest.PatientFirstName,m=>m.MapFrom(src=>src.Patient.User.FirstName))
                    .ForMember(dest=>dest.PatientLastName,m=>m.MapFrom(src=>src.Patient.User.LastName))
                    .ForMember(dest=>dest.DoctorFirstName,m=>m.MapFrom(src=>src.Doctor.User.FirstName))
                    .ForMember(dest=>dest.DoctorLastName,m=>m.MapFrom(src=>src.Doctor.User.LastName))
                    .ReverseMap();
            }

            {
                CreateMap<Doctor, DoctorViewModel>()
                .ForMember(dest => dest.FirstName, o => o.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, o => o.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Specialization, o => o.MapFrom(src => src.Specialization))
                .ForMember(dest => dest.DepartmentName, o => o.MapFrom(src => src.Department.Name))
                .ReverseMap();
            }



            CreateMap<Patient, PatientsHistoryViewModel>()
            .ForMember(dest => dest.FirstName, o => o.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, o => o.MapFrom(src => src.User.LastName))
            .ReverseMap();
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, a => a.MapFrom(src => src.Patient.User.FirstName))
                .ForMember(dest => dest.DoctorName, a => a.MapFrom(src => src.Doctor.User.FirstName))

                .ReverseMap();
            CreateMap<AppointmentDto, AppointmentsViewModel>().ReverseMap();
            CreateMap<AppointmentDto, AppointmentViewModel>().ReverseMap();
            CreateMap<MedicalRecordDto, PatientMedicalRecordsViewModel>().ReverseMap();
            CreateMap<AppointmentDto, PatientAppointmentsViewModel>().ReverseMap();
            CreateMap<ShiftViewModel, ShiftDto>().ReverseMap();
            CreateMap<ShiftDto, Shift>().ReverseMap();

            CreateMap<NextAppointmentDto, NextAppointmentViewModel>().ReverseMap();

        }
    }
}
