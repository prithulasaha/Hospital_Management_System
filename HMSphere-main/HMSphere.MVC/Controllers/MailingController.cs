using HMSphere.Application.Mailing;
using HMSphere.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HMSphere.MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailingController : ControllerBase
    {
        private readonly IMailingService _mailingService;

        public MailingController(IMailingService mailingService)
        {
            _mailingService = mailingService;
        }

        //[HttpPost("send")]
        //public async Task<IActionResult> SendMail([FromForm] MailRequestViewModel mailRequest)
        //{
        //    await _mailingService.SendMailAsync(mailRequest.ToEmail, mailRequest.Subject, mailRequest.Body, mailRequest.Attachments);
        //    return Ok();
        //}
    }
}