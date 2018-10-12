using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace CNMWebApp.Services
{
    public class EmailNotificationService
    {
        public EmailNotificationService()
        {

        }

        public Task SendEmailAsync(string solicitante, string cc, string subject, string body)
        {
            return Task.Run(() =>
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("otistestuh@gmail.com");
                mail.To.Add(solicitante);
                mail.CC.Add(cc);

                mail.Subject = subject;
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential("otistestuh@gmail.com", "OtisTest123");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            });
        }
    }
}