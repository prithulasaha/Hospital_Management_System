using HMSphere.Application.DTOs;

namespace HMSphere.MVC.ViewModels
{
    //public class AssignStaffViewModel
    //{
    //    public int ShiftId { get; set; } 
    //    public List<StaffViewModel> StaffList { get; set; } = new List<StaffViewModel>();
    //    public string SelectedStaffId { get; set; } 
    //}

    //public class AssignDoctorViewModel
    //{
    //    public int ShiftId { get; set; }
    //    public List<DoctorViewModel> DoctorList { get; set; } = new List<DoctorViewModel>();
    //    public string SelectedDoctorId { get; set; } 
    //}
    public class ShiftManagementViewModel
    {
        public List<ShiftViewModel> Shifts { get; set; } = new List<ShiftViewModel>();
        public ShiftDto NewShift { get; set; } = new ShiftDto();
        public List<StaffViewModel> Staff { get; set; } = new List<StaffViewModel>();
        public List<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    }
}
