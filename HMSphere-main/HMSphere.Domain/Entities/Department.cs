using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HMSphere.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public string? Location { get; set; }

        public string? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public virtual Doctor? DeptManager { get; set; } 
        public virtual ICollection<Doctor>? Doctors { get; set; }= new List<Doctor>();
        public virtual ICollection<Staff>? Staff { get; set; }=new List<Staff>();
    }
}