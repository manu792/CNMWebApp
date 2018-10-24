using System;
using System.Collections.Generic;
using System.Configuration;
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
                var smtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpClient"]);

                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailAddress"]);
                mail.To.Add(solicitante);
                mail.CC.Add(cc);

                mail.Subject = subject;
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

                smtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["MailPort"]);
                smtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailAddress"], ConfigurationManager.AppSettings["MailPassword"]);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            });
        }
    }
}