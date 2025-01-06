using AutoMapper;
using HMSphere.Application.Interfaces;
using HMSphere.Domain.Entities;
using HMSphere.MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace HMSphere.MVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffController(IStaffService staffService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _staffService = staffService;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Get staff information (you might already have this mapped to StaffViewModel)
            var staffViewModel = new StaffViewModel
            {
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName
            };

            // Get shifts for the staff member
            var shifts = await _staffService.GetShiftsForStaffAsync(currentUser.Id);
            var shiftViewModels = shifts.Select(s => new ShiftViewModel
            {
                Type = s.Type,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Notes = s.Notes,
                IsActive = s.IsActive
            }).ToList();

            // Create the composite view model
            var viewModel = new StaffWithDetailsShiftsViewModel
            {
                Staff = staffViewModel,
                Shifts = shiftViewModels
            };

            return View("Index", viewModel);
        }

        public async Task<IActionResult> ShiftSchedule()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            var shifts = await _staffService.GetShiftsForStaffAsync(currentUser.Id);
            var ShiftsResult = shifts.Select(s=> _mapper.Map<ShiftViewModel>(s)).ToList();
            return View("ShiftSchedule", ShiftsResult);
        }
    }
}