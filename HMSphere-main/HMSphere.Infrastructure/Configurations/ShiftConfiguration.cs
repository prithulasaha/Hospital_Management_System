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
	public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.Property(p => p.Id)
               .ValueGeneratedOnAdd().IsRequired();

            builder.Property(p => p.Notes)
                .HasMaxLength(200);

            builder.Property(p => p.Type)
                .HasMaxLength(50);

            builder.HasMany(d => d.DoctorShifts)
                .WithOne(ds=>ds.Shift).HasForeignKey(ds => ds.ShiftId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(d => d.StaffShifts)
                .WithOne(ss=>ss.Shift).HasForeignKey(ss => ss.ShiftId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
