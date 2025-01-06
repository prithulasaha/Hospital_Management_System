using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HMSphere.MVC.ViewModels
{
    public class MailRequestViewModel
    {

        [Required]
        public string ToEmail { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }

        public IList<IFormFile> Attachments { get; set; }
     }
}
