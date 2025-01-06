using HMSphere.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace HMSphere.Application.Mailing
{
    public interface IMailingService
    {
        Task SendMailAsync(ApplicationUser user, string subject, string? message);
    }
}
