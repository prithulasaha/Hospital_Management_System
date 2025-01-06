
using HMSphere.Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.AspNetCore.Http;

namespace HMSphere.Application.Mailing
{
    public class MailingService : IMailingService
    {
        private readonly MailSettings _mailSettings;

        public MailingService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public void SendMail(MailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

		private string RegisterBodyGenerator(string userName)
		{
			string body = "<div>";
			body += "<h2>Hello "+userName+",</h2>";
			body += "<h4>Now you are a member of our family <b>HMSphere</b></h4>";
			body += "<br><h5>Have a nice day,</h5>";
			body += "<h6>HMSphere Support Team.</h6>";
			body += "</div>";
			return body;
		}

		private string AppointmentBodyGenerator(string message)
		{
			string body = "<div>";
            body += "<h2>" + message + "</h2>";
			body += "</div>";
			return body;
		}

		public async Task SendMailAsync(ApplicationUser user, string subject,string message)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = subject
            };
            if (user.Email == null)
            {
                return;
            }

            email.To.Add(MailboxAddress.Parse(user.Email));
            var builder = new BodyBuilder();

            if(subject=="Registeration Completed")
            {
                builder.HtmlBody = RegisterBodyGenerator(user.FirstName);
            }
            else
            {
                builder.HtmlBody = AppointmentBodyGenerator(message);
			}

            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);    

            smtp.Disconnect(true);
        }


        private MimeMessage CreateEmailMessage(MailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("HMS", _mailSettings.Email));  // Changed "email" to "sender"
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p><img src=\"cid:logo\" alt=\"HMS\" style=\"vertical-align:middle;\" /> <span style=\"vertical-align:middle;\">Hospital Management System</span></p>" +
                       $"{message.Content}"
            };



            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_mailSettings.Host, _mailSettings.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_mailSettings.DisplayName, _mailSettings.Password);
                    client.Send(message);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }
    }




}
