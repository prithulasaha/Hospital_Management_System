using HMSphere.Application.DTOs;
using HMSphere.Application.Interfaces;
using HMSphere.Domain.Entities;
using HMSphere.Domain.Enums;
using HMSphere.Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Services
{
	public class UserRoleFactory:IUserRoleFactory
	{
		private readonly HmsContext _context;
		public Dictionary<string, (string action, string controller)> roleRedirects
			= new()
			{
				{ "Doctor", ("Index", "Doctor") },
				{ "Patient", ("Index", "Patient") },
				{ "Staff", ("Index", "Staff") },
				{ "Admin", ("Index", "Admin") },
			};
		Dictionary<string, (string action, string controller)> IUserRoleFactory.roleRedirects 
		{ 
			get { return roleRedirects; }
			set { roleRedirects = value; }
		}

		public UserRoleFactory(HmsContext context)
		{
			_context = context;
		}

		public async Task CreateUserEntity(RegisterDto dto, string userId)
		{
			if (dto.Role == "Doctor")
			{
				var doctor = new Doctor
				{
					Id = userId,
					Specialization = dto.Specialization,
                    DepartmentId=dto.DepartmentId,
                };
				await _context.Doctors.AddAsync(doctor);
			}
			else if (dto.Role == "Patient")
			{
				var patient = new Patient
				{
					Id = userId,
					Blood=dto.Blood,
					DiseaseHistory=dto.DiseaseHistory,
					Weight=dto.Weight,
					Height=dto.Height,
				};
				await _context.Patients.AddAsync(patient);
			}
			else if (dto.Role == "Staff")
			{
				var staff = new Staff
				{
					Id = userId,
					Role=Role.OtherStaff,
					JobTitle=dto.JobTitle,
					DepartmentId = dto.DepartmentId
				};
				await _context.Staff.AddAsync(staff);
			}
			else if (dto.Role == "Admin")
			{
				var staff = new Staff
				{
					Id = userId,
					Role = Role.Admin,
					JobTitle = dto.JobTitle
				};
				await _context.Staff.AddAsync(staff);
			}

			await _context.SaveChangesAsync();
		}
	}
}
