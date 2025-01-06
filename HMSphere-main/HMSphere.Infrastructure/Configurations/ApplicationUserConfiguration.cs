using HMSphere.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Infrastructure.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(p => p.FirstName)
                .HasMaxLength(50);

            builder.Property(p => p.LastName)
                .HasMaxLength(50);

            builder.Property(p => p.NID)
                .HasMaxLength(14);


            builder.Property(p => p.Gender)
                .HasMaxLength(10);


            builder.Property(p => p.Address)
			.HasMaxLength(50);

			builder.HasOne(a => a.Patient)
			.WithOne(p => p.User)
			.HasForeignKey<Patient>(p => p.Id)
			.IsRequired(false);

			builder.HasOne(a => a.Doctor)
			.WithOne(p => p.User)
			.HasForeignKey<Doctor>(p => p.Id)
			.IsRequired(false);

			builder.HasOne(a => a.Staff)
			.WithOne(p => p.User)
			.HasForeignKey<Staff>(p => p.Id)
			.IsRequired(false);

		}
    }
}
