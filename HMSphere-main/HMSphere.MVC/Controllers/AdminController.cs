using AutoMapper;
using HMSphere.Application.Interfaces;
using HMSphere.MVC.ViewModels;
using HMSphere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HMSphere.Application.DTOs;
using HMSphere.Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using HMSphere.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace HMSphere.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IStaffService _staffService;
        private readonly IShiftService _shiftService;
        private readonly IBaseRepository<Shift> _shiftRepo;
        private readonly IBaseRepository<Staff> _staffRepo;
        private readonly IBaseRepository<Doctor> _doctorRepo;
        private readonly IBaseRepository<Patient> _patientRepo;
        private readonly IBaseRepository<Department> _deptRepo;
        private readonly UserManager<ApplicationUser> _userManager;


        public AdminController(IAppointmentService appointmentService, IMapper mapper, IBaseRepository<Shift> shiftRepo,
            IBaseRepository<Staff> staffRepo, IBaseRepository<Doctor> docotrRepo, IBaseRepository<Patient> patientRepo,
            IPatientService patientService, IDoctorService doctorService, IStaffService staffService,
            UserManager<ApplicationUser> userManager, IBaseRepository<Department> deptRepo, IShiftService shiftService)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _patientService = patientService;
            _doctorService = doctorService;
            _staffService = staffService;
            _shiftRepo = shiftRepo;
            _doctorRepo = docotrRepo;
            _staffRepo = staffRepo;
            _patientRepo = patientRepo;
            _userManager = userManager;
            _deptRepo = deptRepo;
            _shiftService = shiftService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }
            var response = await _staffService.Profile(userId);
            if (response.IsSuccess)
            {
                var admin = _mapper.Map<StaffViewModel>(response.Model);
                if (admin != null)
                {
                    return View(admin);
                }
                return NotFound();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(int id, bool isApproved)
        {
            var result = await _appointmentService.ApproveAppointment(id, isApproved);
            if (!result)
            {
                // Handle error (e.g., appointment not found)
                return NotFound();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _doctorService.GetAll();
            if (!doctors.Any())
            {
                return View();
            }
            var model = doctors.Select(d => _mapper.Map<DoctorViewModel>(d)).ToList();
            return View(model);
        }

        public async Task<IActionResult> PendingAppointments()
        {
            var pendingAppointments = await _appointmentService.GetPendingAppointments();

            var appointmentsViewModels = _mapper.Map<List<AppointmentViewModel>>(pendingAppointments);

            return View(appointmentsViewModels);
        }

        public async Task<IActionResult> Patients()
        {
            var patients = await _patientService.GetAll();
            if (!patients.Any())
            {
                return View();
            }
            var model = patients.Select(p => _mapper.Map<PatientsHistoryViewModel>(p)).ToList();
            return View(model);
        }

        public async Task<IActionResult> Staff()
        {
            var staff = await _staffService.GetAllAsync();
            if (!staff.Any())
            {
                return View();
            }
            var model = staff.Select(s => _mapper.Map<StaffViewModel>(s)).ToList();
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
                return NotFound("No medical records found for the provided staff ID.");
            }
            var model = medicalRecords.Select(m => _mapper.Map<MedicalRecordViewModel>(m)).ToList();
            return View(model);
        }
        public IActionResult AddDoctor()
        {
            return View();
        }
        public async Task<IActionResult> CreateAppointment()
        {
            ViewData["Patients"] = new SelectList(await _patientService.GetPatients(), "Id", "User.UserName");

            ViewData["Doctors"] = new SelectList(await _doctorService.GetDoctorsByDepartmentIdAsync(null), "Id", "User.UserName");


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointmentDto = _mapper.Map<AppointmentDto>(model);

                var result = await _appointmentService.CreateAppointmentByAdmin(appointmentDto);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View("CreateAppointment", model);
                }

                return RedirectToAction("Appointments");
            }
            ViewData["Patients"] = new SelectList(await _patientService.GetAll(), "Id", "User.UserName");
            ViewData["Doctors"] = new SelectList(await _doctorService.GetDoctorsByDepartmentIdAsync(model.DepartmentId), "Id", "User.UserName");

            return View("CreateAppointment", model);



        }

        // -------------------------------- Doctor Management -------------------------------------------- \\
        public async Task<IActionResult> UpdateDoctor(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            var departments = await _deptRepo.GetAllAsync();

            var doctorViewModel = _mapper.Map<DoctorViewModel>(doctor);
            var user = await _userManager.FindByIdAsync(doctor.Id);

            if (user != null)
            {
                doctorViewModel.FirstName = user.FirstName;
                doctorViewModel.LastName = user.LastName;
                doctorViewModel.PhoneNumber = user.PhoneNumber;
            }
            doctorViewModel.Departments = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = d.Id == doctor.DepartmentId
            }).ToList();

            return View(doctorViewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdateDoctor(string id, DoctorViewModel doctorViewModel)
        {
            if (id != doctorViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingDoctor = await _doctorRepo.GetByIdAsync(id);
                if (existingDoctor == null)
                {
                    return NotFound();
                }

                existingDoctor.DepartmentId = doctorViewModel.DepartmentId;
                existingDoctor.Specialization = doctorViewModel.Specialization;

                await _doctorRepo.UpdateAsync(existingDoctor);

                var user = await _userManager.FindByIdAsync(existingDoctor.Id);
                if (user != null)
                {
                    user.FirstName = doctorViewModel.FirstName;
                    user.LastName = doctorViewModel.LastName;
                    user.PhoneNumber = doctorViewModel.PhoneNumber;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(doctorViewModel);
                    }
                }

                return RedirectToAction("Doctors");
            }

            return View("Doctors");
        }

        public async Task<IActionResult> DetailsDoctor(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            // Assuming the doctor has a property 'DepartmentId' to retrieve its department
            var department = await _deptRepo.GetByIntIdAsync((int)doctor.DepartmentId); // Get the specific department
            if (department == null)
            {
                return NotFound();
            }

            var doctorViewModel = _mapper.Map<DoctorViewModel>(doctor);
            var user = await _userManager.FindByIdAsync(doctor.Id);

            if (user != null)
            {
                doctorViewModel.FirstName = user.FirstName;
                doctorViewModel.LastName = user.LastName;
                doctorViewModel.PhoneNumber = user.PhoneNumber;
            }

            // Set the specific department in the view model
            doctorViewModel.DepartmentName = department.Name; // Assuming you want to display the name directly
            doctorViewModel.DepartmentId = department.Id; // Store the department ID if needed

            return View(doctorViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(doctor.Id);
            if (user != null)
            {
                await _doctorRepo.DeleteAsync(doctor);
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to delete associated user.");
                    return RedirectToAction("Doctors");
                }
            }
            TempData["Message"] = "Doctor and associated user deleted successfully.";
            return RedirectToAction("Doctors");
        }

        // -------------------------------- Staff Management -------------------------------------------- \\

        public async Task<IActionResult> UpdateStaff(string? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var staff = await _staffRepo.GetByIdAsync(id);
			if (staff == null)
			{
				return NotFound();
			}

			var departments = await _deptRepo.GetAllAsync();

			var staffViewModel = _mapper.Map<StaffViewModel>(staff);
			var user = await _userManager.FindByIdAsync(staff.Id);

			if (user != null)
			{
				staffViewModel.FirstName = user.FirstName;
				staffViewModel.LastName = user.LastName;
				staffViewModel.PhoneNumber = user.PhoneNumber;
			}
			staffViewModel.Departments = departments.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.Name,
				Selected = d.Id == staff.DepartmentId
			}).ToList();

			return View(staffViewModel);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SaveUpdateStaff(string id, StaffViewModel staffViewModel)
		{
			if (id != staffViewModel.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				var existingStaff = await _staffRepo.GetByIdAsync(id);
				if (existingStaff == null)
				{
					return NotFound();
				}

				existingStaff.DepartmentId = staffViewModel.DepartmentId;
				existingStaff.JobTitle = staffViewModel.JobTitle;
                existingStaff.HireDate = staffViewModel.HireDate;

				await _staffRepo.UpdateAsync(existingStaff);

				var user = await _userManager.FindByIdAsync(existingStaff.Id);
				if (user != null)
				{
					user.FirstName = staffViewModel.FirstName;
					user.LastName = staffViewModel.LastName;
					user.PhoneNumber = staffViewModel.PhoneNumber;

					var result = await _userManager.UpdateAsync(user);
					if (!result.Succeeded)
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
						return View(staffViewModel);
					}
				}

				return RedirectToAction("Staff");
			}

			return View("Staff");
		}


		public async Task<IActionResult> DetailsStaff(string? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var staff = await _staffRepo.GetByIdAsync(id);
			if (staff == null)
			{
				return NotFound();
			}

			// Assuming the doctor has a property 'DepartmentId' to retrieve its department
			var department = await _deptRepo.GetByIntIdAsync((int)staff.DepartmentId); // Get the specific department
			if (department == null)
			{
				return NotFound();
			}

			var staffViewModel = _mapper.Map<StaffViewModel>(staff);
			var user = await _userManager.FindByIdAsync(staff.Id);

			if (user != null)
			{
				staffViewModel.FirstName = user.FirstName;
				staffViewModel.LastName = user.LastName;
				staffViewModel.PhoneNumber = user.PhoneNumber;
			}

			// Set the specific department in the view model
			staffViewModel.DepartmentName = department.Name; // Assuming you want to display the name directly
			staffViewModel.DepartmentId = department.Id; // Store the department ID if needed

			return View(staffViewModel);
		}

		[HttpPost]
        public async Task<IActionResult> DeleteStaff(string id)
        {
			var staff = await _staffRepo.GetByIdAsync(id);
			if (staff == null)
			{
				return NotFound();
			}

			var user = await _userManager.FindByIdAsync(staff.Id);
			if (user != null)
			{
				await _staffRepo.DeleteAsync(staff);
				var result = await _userManager.DeleteAsync(user);
				if (!result.Succeeded)
				{
					ModelState.AddModelError("", "Failed to delete associated user.");
					return RedirectToAction("Staff");
				}
			}
			TempData["Message"] = "Staff and associated user deleted successfully.";
			return RedirectToAction("Staff");
		} 
        public IActionResult UpdatePatient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePatient(string id)
        {
            var patient = await _patientRepo.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            await _patientRepo.DeleteAsync(patient);
            TempData["Message"] = "Patient deleted successfully.";

            return RedirectToAction("Patients");
        }

        [HttpPost]
        public async Task<IActionResult> AddShift(ShiftDto newShift)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Shifts");
            }

            var shiftEntity = _mapper.Map<Shift>(newShift);
            await _shiftRepo.AddAsync(shiftEntity);
            return RedirectToAction("Shifts");
        }

        [HttpPost]
        public async Task<IActionResult> SaveAssignStaffToShift(int shiftId, string staffId)
        {
            if (string.IsNullOrEmpty(staffId))
            {
                ModelState.AddModelError("", "Please select a one of Staff to assign.");
                return RedirectToAction("Shifts");
            }

            // Attempt to assign the doctor to the shift
            bool result = await _shiftService.AssignStaffToShiftAsync(shiftId, staffId);

            if (!result)
            {
                // Doctor is already assigned to this shift; show a message to the admin
                TempData["Message"] = "The selected Staff is Already assigned to this Shift.";
                return RedirectToAction("Shifts");
            }

            TempData["Message"] = "Staff assigned successfully.";
            return RedirectToAction("Shifts");
        }

        [HttpPost]
        public async Task<IActionResult> SaveAssignDoctorToShift(int shiftId, string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                ModelState.AddModelError("", "Please select a staff to assign.");
                return RedirectToAction("Shifts");
            }

            // Attempt to assign the doctor to the shift
            bool result = await _shiftService.AssignDoctorToShiftAsync(shiftId, doctorId);

            if (!result)
            {
                // Doctor is already assigned to this shift; show a message to the admin
                TempData["Message"] = "The selected Doctor is Already assigned to this Shift.";
                return RedirectToAction("Shifts");
            }

            TempData["Message"] = "Doctor assigned successfully.";
            return RedirectToAction("Shifts");
        }
        public async Task<ActionResult> Shifts()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            var doctorViewModels = doctors.Select(doctor => _mapper.Map<DoctorViewModel>(doctor)).ToList();

            var Staff = await _staffService.GetAllAsync();
            var StaffViewModels = Staff.Select(staff => _mapper.Map<StaffViewModel>(staff)).ToList();

            var shifts = await _shiftRepo.GetAllAsync();
            var shiftDtos = shifts.Select(shift => _mapper.Map<ShiftDto>(shift)).ToList();

            var shiftViewModels = shiftDtos.Select(shift => _mapper.Map<ShiftViewModel>(shift)).ToList();

            foreach (var doctor in doctorViewModels)
            {
                var detailedDoctor = await _doctorRepo.GetByIdAsync(doctor.Id);
                var doctorViewModel = _mapper.Map<DoctorViewModel>(doctor);
                var user = await _userManager.FindByIdAsync(doctor.Id);
                if (user != null)
                {
                    doctor.FirstName = user.FirstName;
                    doctor.LastName = user.LastName;
                    doctor.PhoneNumber = user.PhoneNumber;
                }
                
            }
            foreach(var staff in StaffViewModels)
            {
                var detailedStaff = await _staffRepo.GetByIdAsync(staff.Id);
                var doctorViewModel = _mapper.Map<StaffViewModel>(staff);
                var user = await _userManager.FindByIdAsync(staff.Id);
                if (user != null)
                {
                    staff.FirstName = user.FirstName;
                    staff.LastName = user.LastName;
                    staff.PhoneNumber = user.PhoneNumber;
                }
            }
            var model = new ShiftManagementViewModel
            {
                Shifts = shiftViewModels,
                NewShift = new ShiftDto(),
                Doctors = doctorViewModels,
                Staff = StaffViewModels,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShift(int id)
        {
            var shift = await _shiftRepo.GetByIntIdAsync(id); // Fetch by ID
            if (shift == null)
            {
                return NotFound(); // Handle not found
            }
            await _shiftRepo.DeleteAsync(shift); // Delete the shift    
            TempData["Message"] = "Shift deleted successfully.";

            return RedirectToAction("Shifts"); // Redirect after deletion
        }
    }
}
