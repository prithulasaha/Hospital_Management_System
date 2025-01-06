using E_Commerce.Domain.Entities;
using HMSphere.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HMSphere.Infrastructure.DataContext
{
    public class HmsContext:IdentityDbContext<ApplicationUser>
    {
        public HmsContext(DbContextOptions<HmsContext> options) : base(options) { }
        public HmsContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<DoctorShift> DoctorShifts { get; set; }
        public DbSet<StaffShift> StaffShifts { get; set; }
	}
}
