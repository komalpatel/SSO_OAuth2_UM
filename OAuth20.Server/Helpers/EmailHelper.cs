using OAuth20.Server.EmailTemplates;
using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net.Http;
using System.Net.Mail;

namespace OAuth20.Server.Helpers
{
    public class EmailHelper
    {

        public bool SendEmailPasswordReset(string userEmail, string link)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@chargeatfriends.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            string filePath = "EmailTemplates\\Welcome.html";

            string emailTemplateText = File.ReadAllText(filePath);

            //var htmlBody = string.Format(emailTemplateText, welcome.EmailToName,welcome.RegistrationLink DateTime.Today.Date.ToShortDateString());

            Welcome welcome = new Welcome();
            welcome.UserName = userEmail;
            welcome.RegistrationLink = link;

            var htmlBody = string.Format(emailTemplateText, welcome.UserName, welcome.RegistrationLink);


            mailMessage.Subject = "Password Reset";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlBody;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("noreply@chargeatfriends.com", "F880704A-6A86-$AD0-A94A");
            client.Host = "smtps.udag.de";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.SendCompleted += (s, e) =>
            {
                mailMessage.Dispose();
                client.Dispose();
                if (!e.Cancelled && e.Error == null)
                {

                }
                else
                {

                }
            };


            try
            {
                client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }
        public bool SendEmailRegister(string userEmail)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@chargeatfriends.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            string filePath = "EmailTemplates\\Register.html";

            string emailTemplateText = File.ReadAllText(filePath);

            //var htmlBody = string.Format(emailTemplateText, welcome.EmailToName,welcome.RegistrationLink DateTime.Today.Date.ToShortDateString());

            var htmlBody = string.Format(emailTemplateText, userEmail);


            mailMessage.Subject = "Welcome for register with us!";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlBody;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("noreply@chargeatfriends.com", "F880704A-6A86-$AD0-A94A");
            client.Host = "smtps.udag.de";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.SendCompleted += (s, e) =>
            {
                mailMessage.Dispose();
                client.Dispose();
                if (!e.Cancelled && e.Error == null)
                {

                }
                else
                {

                }
            };


            try
            {
                client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }


        public bool SendEmailTwoFactorCode(string userEmail, string code)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@chargeatfriends.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            string filePath = "EmailTemplates\\TwoFactor.html";

            string emailTemplateText = File.ReadAllText(filePath);

            //var htmlBody = string.Format(emailTemplateText, welcome.EmailToName,welcome.RegistrationLink DateTime.Today.Date.ToShortDateString());

            var htmlBody = string.Format(emailTemplateText, userEmail, code);


            mailMessage.Subject = "Your Two Factor Authentication code :";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlBody;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("noreply@chargeatfriends.com", "F880704A-6A86-$AD0-A94A");
            client.Host = "smtps.udag.de";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.SendCompleted += (s, e) =>
            {
                mailMessage.Dispose();
                client.Dispose();
                if (!e.Cancelled && e.Error == null)
                {

                }
                else
                {

                }
            };


            try
            {
                client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }
    }
}