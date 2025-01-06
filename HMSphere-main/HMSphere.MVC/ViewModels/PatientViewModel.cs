namespace HMSphere.MVC.ViewModels
{
    public class PatientViewModel
    {
        public List<AppointmentsViewModel> Last5Appointments {get;set;}
        public List<MedicalRecordViewModel> Last5MedicalRecords { get; set; }
    }
}
