using Attendance.Common.Constants;
using Attendance.Service.IServices;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Attendance.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;

        public EmailService(ILogger<EmailService> logger)
        {
            this.logger = logger;
            this.logger.LogInformation("Create MailService");
        }
        public async Task Send(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(AppSettings.DisplayName, AppSettings.Mail);
            email.From.Add(new MailboxAddress(AppSettings.DisplayName, AppSettings.Mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = html;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient(); // Using finished deleted so as not to slow down the system.

            try
            {
                smtp.Connect(AppSettings.Host, AppSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(AppSettings.Mail, AppSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            smtp.Disconnect(true);

            logger.LogInformation("send mail to " + to);
        }
    }
}
