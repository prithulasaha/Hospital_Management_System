namespace HMSphere.MVC.ViewModels
{
    public class StaffWithDetailsShiftsViewModel
    {
        public StaffViewModel Staff { get; set; }
        public List<ShiftViewModel> Shifts { get; set; } = new List<ShiftViewModel>();

    }
}
