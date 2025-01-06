using HMSphere.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSphere.Application.Interfaces
{
	public interface IUserRoleFactory
	{
		Task CreateUserEntity(RegisterDto dto,string userId);
		Dictionary<string, (string action, string controller)> roleRedirects { get; set; }
	}
}
