using AutoMapper;
using HMSphere.Application.DTOs;
using HMSphere.Application.Interfaces;
using HMSphere.Application.Services;
using HMSphere.Domain.Entities;
using HMSphere.Infrastructure.Repositories;
using HMSphere.MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMSphere.MVC.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
		private readonly IPatientService _patientService;
		private readonly IBaseRepository<MedicalRecord> _baseRepository;
		private readonly IMapper _mapper;
		private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;


        public DoctorController(IDoctorService doctorService, IBaseRepository<MedicalRecord> baseRepository, IMapper mapper, IAppointmentService appointmentService, UserManager<ApplicationUser> userManager, IPatientService patientService)
        {
            _doctorService = doctorService;
            _baseRepository = baseRepository;
            _mapper = mapper;
            _appointmentService = appointmentService;
            _userManager = userManager;
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }
            var response = await _doctorService.Profile(userId);
            if (response.IsSuccess)
            {
                var doctor = _mapper.Map<DoctorViewModel>(response.Model);
				var latestAppointments= await GetLatestAppointmentsModel(userId);
				var latestRecords = await GetLatestMedicalRecords(userId);
                if (doctor != null)
				{
					var appoints = await _doctorService.GetNext7DaysAppointments(userId);
					var patients=await _doctorService.GetNumberOfPatients(userId);
					var records=await _doctorService.GetNumberOfMedicalRecords(userId);

                    doctor.NumberOfPatients = patients;
					doctor.UpcomingAppointmentsCount = appoints;
					doctor.NumberOfMedicalRecords = records;
					doctor.LatestAppointments= latestAppointments;
					doctor.LatestMedicalRecords = latestRecords;
                    return View(doctor);
                }
                return NotFound();
            }
            return NotFound();
        }

		private async Task<List<AppointmentsViewModel>> GetLatestAppointmentsModel(string id)
		{
			var models=new List<AppointmentsViewModel>();
            var latestAppointmentsModels = await _doctorService.GetLatestAppointments(id);
			foreach (var model in latestAppointmentsModels)
			{
                var latestAppointment = _mapper.Map<AppointmentsViewModel>(model);
				models.Add(latestAppointment);
            }
			return models;
        }
		private async Task<List<MedicalRecordViewModel>> GetLatestMedicalRecords(string id)
		{
            var models = new List<MedicalRecordViewModel>();
            var latestMedicalRecordsModels = await _doctorService.GetLatestMedicalRecords(id);
            foreach (var model in latestMedicalRecordsModels)
            {
                var latestRecord = _mapper.Map<MedicalRecordViewModel>(model);
                models.Add(latestRecord);
            }
            return models;
        }

        public async Task<IActionResult> AppointmentDetails(int appointmentId)
        {
			var response=await _doctorService.GetAppointmentDetails(appointmentId);
			if (response.IsSuccess)
			{
				var model = _mapper.Map<AppointmentsViewModel>(response.Model); 
				return View(model);
			}
            return NotFound();
        }

		public async Task<IActionResult> PatientHistory()
		{
			var currentUser=await _userManager.GetUserAsync(User);
			if(currentUser == null)
			{
				return NotFound();
			}

			var patients = await _doctorService.GetAllPatientAsync(currentUser.Id);
			var model=patients.Select(p=>_mapper.Map<PatientsHistoryViewModel>(p)).ToList();

			return View(model);
		}

		public async Task<IActionResult> Appointments(string id)
		{
            var currentUser = await _userManager.GetUserAsync(User);
			if(currentUser == null)
			{
                return NotFound();
            }
			var appoints = await _doctorService.GetAllAppointments(currentUser.Id);
			var model=appoints.Select(a=>_mapper.Map<AppointmentsViewModel>(a)).ToList();

            return View(model);
		}


		public async Task<IActionResult> MedicalRecords(string? patientId)
		{
			if (string.IsNullOrEmpty(patientId))
			{
				return BadRequest("Patient ID is required.");
			}

			var medicalRecords = await _doctorService.GetAllMedicalRecordsAsync(patientId);
			if (medicalRecords == null || !medicalRecords.Any())
			{
				return NotFound("No medical records found for the provided patient ID.");
			}
			var model = medicalRecords.Select(m => _mapper.Map<MedicalRecordViewModel>(m)).ToList();
			return View(model);
		}

		public async Task<IActionResult> MedicalRecordDetails(int recordId)
		{
			var record = await _doctorService.GetMedicalRecordDetails(recordId);
			if (record.IsSuccess)
			{
				var model=_mapper.Map<MedicalRecordViewModel>(record.Model);
				return View(model);
			}
			return NotFound();
		}

		[HttpGet]
		public async Task<IActionResult> SearchPatientByName(string name)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			List<PatientDto> patients = new();
			if (string.IsNullOrEmpty(name))
			{
				patients = await _doctorService.GetAllPatientAsync(currentUser.Id);
			}
			else
			{
				patients = await _patientService.SearchByName(name, currentUser.Id);
			}
			var model = patients.Select(p => _mapper.Map<PatientsHistoryViewModel>(p)).ToList();
			return Json(model);
		}

        [HttpGet]
        public async Task<IActionResult> SearchPatientByNID(string nid)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            List<PatientDto> patients = new();
            if (string.IsNullOrEmpty(nid))
            {
                patients = await _doctorService.GetAllPatientAsync(currentUser.Id);
            }
            else
            {
                patients = await _patientService.SearchByNID(nid, currentUser.Id);
            }
            var model = patients.Select(p => _mapper.Map<PatientsHistoryViewModel>(p)).ToList();
            return Json(model);
        }

		[HttpGet]
		public async Task<IActionResult> SearchPatientByBloodType(string type)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			List<PatientDto> patients = new();
			if (string.IsNullOrEmpty(type))
			{
				patients = await _doctorService.GetAllPatientAsync(currentUser.Id);
			}
			else
			{
				patients = await _patientService.SearchByBloodType(type, currentUser.Id);
			}
			var model = patients.Select(p => _mapper.Map<PatientsHistoryViewModel>(p)).ToList();
			return Json(model);
		}

		[HttpGet]
		public async Task<IActionResult> SearchAppointmentByStatus(string status)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			List<AppointmentDto> appointments = new();
			if (string.IsNullOrEmpty(status)||status=="All")
			{
				appointments = await _doctorService.GetAllAppointments(currentUser.Id);
			}
			else
			{
				appointments = await _appointmentService.SearchByStatus(status, currentUser.Id);
			}
			var model = appointments.Select(a => _mapper.Map<AppointmentsViewModel>(a)).ToList();
			return Json(model);
		}
	}
}
