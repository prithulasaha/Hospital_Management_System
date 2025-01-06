using HMSphere.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Infrastructure.Configurations
{
	public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd().IsRequired();

  
            builder.Property(p => p.Diagnosis)
                .HasMaxLength(100);


            builder.Property(p => p.TreatmentPlan)
               .HasMaxLength(500);

            builder.Property(p => p.Medications)
               .HasMaxLength(200);

            builder.Property(p => p.DoctorNotes)
                .HasMaxLength(500);

        }
    }
}
