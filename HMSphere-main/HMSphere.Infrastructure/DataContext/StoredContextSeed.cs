using HMSphere.Domain.Entities;
using HMSphere.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HMSphere.Infrastructure.DataContext
{
    public class StoredContextSeed
    {
        public static async Task SeedAsync(HmsContext context)
        {
            if (!context.Departments.Any())
            {
                var departmentsData = File.ReadAllText("../HMSphere.Infrastructure/SeedData/Departments.json");
                var departments = JsonSerializer.Deserialize<List<Department>>(departmentsData);
                foreach (var item in departments)
                {
                    context.Departments.Add(item);
                }
                await context.SaveChangesAsync();
            }
        }

        public static async Task AppointmentSeed(HmsContext context)
        {
            if (!context.Appointments.Any())
            {
                var data = File.ReadAllText("../HMSphere.Infrastructure/SeedData/Appointments.json");
                var appoints = JsonSerializer.Deserialize<List<Appointment>>(data);
                foreach (var item in appoints)
                {
                    context.Appointments.Add(item);
                }
                //await context.SaveChangesAsync();
            }
        }

        //public static async Task AdminSeed(HmsContext context,
        //    UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        //{
        //    if (!context.Staff.Any(s => s.Role == Role.Admin))
        //    {
        //        var admin = new ApplicationUser
        //        {
        //            FirstName = "Mohamed",
        //            LastName = "Ali",
        //            Email = "admin@gmail.com",
        //            UserName = "mohdali",
        //            Address = "Egypt-Qena",
        //            NID = "12345678912345",
        //            Gender = "Male",
        //        };

        //        await userManager.CreateAsync(admin, "Admin@123");
        //        if (!await roleManager.RoleExistsAsync("Admin"))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole("Admin"));
        //        }
        //        await userManager.AddToRoleAsync(admin, "Admin");

        //        //var staff = new Staff
        //        //{
        //        //    Id= "5748ac57-5864-4f58-b54b-3bc953095424",
        //        //    Role = Role.Admin,
        //        //    JobTitle = "Department Manager",
        //        //    DepartmentId = 6,
        //        //};
        //        //await context.Staff.AddAsync(staff);
        //        //await context.SaveChangesAsync();
        //    }
        //}


    //    public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, HmsContext context)
    //    {
    //        if (!userManager.Users.Any())
    //        {
    //            //create new users
    //            var user = new ApplicationUser
    //            {
    //                FirstName = "Mohamed",
    //                LastName = "Ali",
    //                Email = "mohdali30060@gmail.com",
    //                UserName = "Mohamed_Ali",
    //                Address = "Egypt-Qena"
    //            };
    //            await userManager.CreateAsync(user, "Asd123@");
    //            await userManager.AddToRoleAsync(user, "Doctor");

    //            var doctor = new Doctor
    //            {
    //                Id = user.Id,
    //                Specialization = "General surgery",
    //                DepartmentId = 2,
    //            };
    //            await context.Doctors.AddAsync(doctor);
    //            await context.SaveChangesAsync();
    //        }
    //        if (!userManager.Users.Any())
    //        {
    //            //create new users
    //            var user = new ApplicationUser
    //            {
    //                FirstName = "Ahmed",
    //                LastName = "Naser",
    //                Email = "ahmednasser@gmail.com",
    //                UserName = "Ahmed_Nasser",
    //                Address = "Egypt-Qena"
    //            };
    //            await userManager.CreateAsync(user, "Asd123@");
    //            await userManager.AddToRoleAsync(user, "Doctor");

    //            var doctor = new Doctor
    //            {
    //                Id = user.Id,
    //                Specialization = "General surgery",
    //                DepartmentId = 2,
    //            };
    //            await context.Doctors.AddAsync(doctor);
    //            await context.SaveChangesAsync();
    //        }
    //        if (!userManager.Users.Any())
    //        {
    //            //create new users
    //            var user = new ApplicationUser
    //            {
    //                FirstName = "Eman",
    //                LastName = "Hamam",
    //                Email = "emanhamam109@gmail.com",
    //                UserName = "Eman_Hamam",
    //                Address = "Egypt-Qena"
    //            };
    //            await userManager.CreateAsync(user, "Asd123@");
    //            await userManager.AddToRoleAsync(user, "Patient");

    //            var patient = new Patient
    //            {
    //                Id = user.Id,
    //                Blood = "A+",
    //                Height = 155,
    //                Weight = 50
    //            };
    //            await context.Patients.AddAsync(patient);
    //            await context.SaveChangesAsync();
    //        }
    //        if (!userManager.Users.Any())
    //        {
    //            //create new users
    //            var user = new ApplicationUser
    //            {
    //                FirstName = "Nermen",
    //                LastName = "Ashraf",
    //                Email = "nermin@gmail.com",
    //                UserName = "Nermin_Ashraf",
    //                Address = "Egypt-Qena"
    //            };
    //            await userManager.CreateAsync(user, "Asd123@");
    //            await userManager.AddToRoleAsync(user, "Staff");

    //            var staff = new Staff
    //            {
    //                Id = user.Id,
    //                JobTitle = "nurse",
    //                DepartmentId = 2,
    //            };
    //            await context.Staff.AddAsync(staff);
    //            await context.SaveChangesAsync();
    //        }
    //        if (!userManager.Users.Any())
    //        {
    //            //create new users
    //            var user = new ApplicationUser
    //            {
    //                FirstName = "Rawan",
    //                LastName = "Adel",
    //                Email = "Rawan@gmail.com",
    //                UserName = "Rawan_Adel",
    //                Address = "Egypt-Qena"
    //            };
    //            await userManager.CreateAsync(user, "Asd123@");
    //            await userManager.AddToRoleAsync(user, "Patient");

    //            var patient = new Patient
    //            {
    //                Id = user.Id,
    //                Blood = "A+",
    //                Height = 155,
    //                Weight = 50
    //            };
    //            await context.Patients.AddAsync(patient);
    //            await context.SaveChangesAsync();

    //        }
    //        if (!userManager.Users.Any())
    //        {
    //            //create new users
    //            var user = new ApplicationUser
    //            {
    //                FirstName = "Nahla",
    //                LastName = "",
    //                Email = "nahla@gmail.com",
    //                UserName = "Nahla",
    //                Address = "Egypt-Qena"
    //            };
    //            await userManager.CreateAsync(user, "Asd123@");
    //            await userManager.AddToRoleAsync(user, "Staff");

    //            var staff = new Staff
    //            {
    //                Id = user.Id,
    //                JobTitle = "nurse",
    //                DepartmentId = 2,
    //            };
    //            await context.Staff.AddAsync(staff);
    //            await context.SaveChangesAsync();
    //        }
    //    }
     }
}

