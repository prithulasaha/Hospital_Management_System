using HMSphere.Application.Interfaces;
using HMSphere.Domain.Entities;
using HMSphere.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HmsContext _context;
        public DepartmentService(HmsContext context)
        {
            _context = context;
        }
        public async Task<List<Department>> GetDepartments()
        {
            return await _context.Departments.Include(d=>d.DeptManager).ToListAsync();
        }
    }
}
