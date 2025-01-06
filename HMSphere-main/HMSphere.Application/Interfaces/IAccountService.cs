using HMSphere.Application.DTOs;
using HMSphere.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ApplicationUser> GetCurrentUser(string email);
		Task<AuthDto> RegisterAsync(RegisterDto model);
        Task<AuthDto> LoginAsync(LoginDto model);
        Task<AuthDto> LogoutAsync(string email);

    }
}
