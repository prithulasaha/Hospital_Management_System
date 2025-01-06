using HMSphere.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Infrastructure.Configurations
{
	public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.Property(p => p.Blood)
                .IsRequired().HasMaxLength(5);


            builder.Property(p => p.DiseaseHistory)
               .HasMaxLength(200);

			builder.HasMany(d => d.Appointments)
				.WithOne(a => a.Patient)
				.HasForeignKey(a => a.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p=>p.MedicalRecords)
                .WithOne(m=>m.Patient).HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

		}
    }
}
