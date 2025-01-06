using AutoMapper;
using HMSphere.Application.DTOs;
using HMSphere.Application.Interfaces;
using HMSphere.Application.Services;
using HMSphere.Domain.Entities;
using HMSphere.Domain.Enums;
using HMSphere.MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Numerics;
using System.Security.Claims;

namespace HMSphere.MVC.Controllers
{
    public class PatientController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IDepartmentService _departmentService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly IPatientService _patientService;

        private readonly UserManager<ApplicationUser> _userManager;

        public PatientController(IDoctorService doctorService,
                                IDepartmentService departmentService,
                                IAppointmentService appointmentService,
                                IMapper mapper,
                                UserManager<ApplicationUser> userManager,
                                IPatientService patientService)
        {
            _doctorService = doctorService;
            _departmentService = departmentService;
            _appointmentService = appointmentService;
            _mapper = mapper;
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
            var response = await _patientService.Profile(userId);
            if (response.IsSuccess)
            {
                var patient = _mapper.Map<PatientsHistoryViewModel>(response.Model);
                var latestRecords = await GetLatestMedicalRecords(userId);
                var latestAppointments = await GetLatestAppointmentsModel(userId);
                if (patient != null)
                {
                    var nextappointment=await _patientService.GetNextAppointmentByPatientIdAsync(userId);

                    patient.NextAppointment=_mapper.Map<NextAppointmentViewModel>(nextappointment);
                    patient.LatestAppointments = latestAppointments;
                    patient.LatestMedicalRecords = latestRecords;
                    return View(patient);
                }
                return NotFound();
            }
            return NotFound();
        }
        public async Task<IActionResult> Appointments()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User is not authenticated.");
            }

            var appointmentDtos = await _appointmentService.GetAllAppointmentsByPatientIdAsync(userId);

           /* if (appointmentDtos == null || !appointmentDtos.Any())
            {
                return View();
            }*/

            var appointmentViewModels = appointmentDtos.Select(dto => new AppointmentViewModel
            {
                Id = dto.Id,
                Date = dto.Date,
                Status = dto.Status,
                ReasonFor = dto.ReasonFor,
                Clinic = dto.Clinic,
                PatientId = dto.PatientId,
                DepartmentId = dto.DepartmentId,
                AppointmentTime = dto.AppointmentTime,
                DoctorId = dto.DoctorId,
                PatientName = dto.PatientName,
                DoctorName = dto.DoctorName
            }).ToList();

            return View(appointmentViewModels);
        }
        public async Task<IActionResult> MedicalRecords()
        {
            List<MedicalRecordViewModel> models = new();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            var medicalrecords = await _patientService.GetAllMedicalRecordsAsync(currentUser.Id);
            foreach (var record in medicalrecords)
            {
                var model=_mapper.Map<MedicalRecordViewModel>(record);
                models.Add(model);
            }
            return View(models);
        }

        public async Task<IActionResult> CreateAppointment()
        {
            ViewData["Departments"] = new SelectList(await _departmentService.GetDepartments(), "Id", "Name");
            ViewData["Doctors"] = new SelectList(await _doctorService.GetDoctorsByDepartmentIdAsync(null), "Id", "User.UserName");


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointmentDto = _mapper.Map<AppointmentDto>(model);

                var result = await _appointmentService.CreateAppointment(appointmentDto);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View("CreateAppointment",model);
                }

                return RedirectToAction("Appointments");
            }
            ViewData["Departments"] = new SelectList(await _departmentService.GetDepartments(), "Id", "Name");
            ViewData["Doctors"] = new SelectList(await _doctorService.GetDoctorsByDepartmentIdAsync(model.DepartmentId), "Id", "User.UserName");
           
            return View("CreateAppointment", model);
        }


        public async Task<IActionResult> UpdateAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<AppointmentViewModel>(appointment);

            ViewData["Departments"] = new SelectList(await _departmentService.GetDepartments(), "Id", "Name", model.DepartmentId);
            ViewData["Doctors"] = new SelectList(await _doctorService.GetDoctorsByDepartmentIdAsync(model.DepartmentId), "Id", "User.UserName", model.DoctorId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointmentDto = _mapper.Map<AppointmentDto>(model);
                var result = await _appointmentService.UpdateAppointment(appointmentDto);

                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    ViewData["Departments"] = new SelectList(await _departmentService.GetDepartments(), "Id", "Name", model.DepartmentId);
                    ViewData["Doctors"] = new SelectList(await _doctorService.GetDoctorsByDepartmentIdAsync(model.DepartmentId), "Id", "User.UserName", model.DoctorId);
                    return View(model);
                }

                return RedirectToAction("Appointments");
            }

            ViewData["Departments"] = new SelectList(await _departmentService.GetDepartments(), "Id", "Name", model.DepartmentId);
            ViewData["Doctors"] = new SelectList(await _doctorService.GetDoctorsByDepartmentIdAsync(model.DepartmentId), "Id", "User.UserName", model.DoctorId);

            return View(model);
        }

        public async Task<JsonResult> GetDoctorsByDepartment(int? departmentId)
        {
            var doctors = await _doctorService.GetDoctorsByDepartmentIdAsync(departmentId);
            return Json(new SelectList(doctors, "Id", "User.UserName"));
        }
        public async Task<IActionResult> AppointmentDetails(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid appointment ID.");
            }

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                Date = appointment.Date,
                AppointmentTime = appointment.AppointmentTime,
                PatientName = appointment.PatientName,
                DoctorName = appointment.DoctorName,
                Status = appointment.Status,
                ReasonFor = appointment.ReasonFor,
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var result = await _appointmentService.CancelAppointment(appointmentId);

            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to cancel the appointment.";
            }
            else
            {
                TempData["SuccessMessage"] = "Appointment canceled successfully.";
            }

            // Redirect to the Appointment Details page, or wherever you'd like to redirect the user
            return RedirectToAction("AppointmentDetails", new { id = appointmentId });
        }


        public async Task<IActionResult> MedicalRecordDetails(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid medicalRecord ID.");
            }

            var medicalRecord = await _patientService.GetMedicalRecordAsync(id);

            if (medicalRecord == null)
            {
                return NotFound("medicalRecord not found.");
            }

            var viewModel = new MedicalRecordViewModel
            {
                Id = medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                DoctorId=medicalRecord.DoctorId,
                Diagnosis=medicalRecord.Diagnosis,
                Medications = medicalRecord.Medications,
                TreatmentPlan=medicalRecord.TreatmentPlan,
                DoctorNotes=medicalRecord.DoctorNotes,

            };

            return View(viewModel);
        }

        private async Task<List<MedicalRecordViewModel>> GetLatestMedicalRecords(string id)
        {
            var models = new List<MedicalRecordViewModel>();
            var latestMedicalRecordsModels = await _patientService.GetLast5MedicalRecordsAsync(id);
            foreach (var model in latestMedicalRecordsModels)
            {
                var latestRecord = _mapper.Map<MedicalRecordViewModel>(model);
                models.Add(latestRecord);
            }
            return models;
        }
        private async Task<List<AppointmentViewModel>> GetLatestAppointmentsModel(string id)
        {
            var models = new List<AppointmentViewModel>();
            var latestAppointmentsModels = await _patientService.GetLast5AppointmentsAsync(id);
            foreach (var model in latestAppointmentsModels)
            {
                var latestAppointment = _mapper.Map<AppointmentViewModel>(model);
                models.Add(latestAppointment);
            }
            return models;
        }
    }
}
