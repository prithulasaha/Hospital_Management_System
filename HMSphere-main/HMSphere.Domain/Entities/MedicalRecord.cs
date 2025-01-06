using HMSphere.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Domain.Entities
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.Now;
        public string Diagnosis { get; set; }= string.Empty;
        public string? TreatmentPlan { get; set; }=string.Empty;
        public string Medications { get; set; } = string.Empty;
        public string? DoctorNotes { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string PatientId { get; set; }
        public string DoctorId { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }

    }
}