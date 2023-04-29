using OAuth20.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OAuth20.Server.Services
{
	public interface IEmailSenderExtended
	{
		//
		// Summary:
		//     This API supports the ASP.NET Core Identity default UI infrastructure and is
		//     not intended to be used directly from your code. This API may change or be removed
		//     in future releases.
		Task SendEmailAsyncWAttchment(string email, string subject, string htmlMessage, Stream attachment, string attachmentName);
		Task SendEmailAsync(string email, string subject, string htmlMessage);
	}
	public class EmailSender : IEmailSenderExtended
	{
		private static string SmtpServer = StaticData.Configuration["SmtpServer"];
		public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
		{
			Options = optionsAccessor.Value;
		}

		public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

		public Task SendEmailAsync(string email, string subject, string message)
		{
			return Execute(Options.SendGridKey, subject, message, email);
		}

		public Task SendEmailAsyncWAttchment(string email, string subject, string message, Stream attachment, string attachmentName)
		{
			return Execute(Options.SendGridKey, subject, message, email, attachment, attachmentName);
		}

		//public Task Execute(string apiKey, string subject, string message, string email)
		//{
		//	var client = new SendGridClient(apiKey);
		//	var msg = new SendGridMessage()
		//	{
		//		From = new EmailAddress("noreply@chargeatfriends.com", Options.SendGridUser),
		//		Subject = subject,
		//		PlainTextContent = message,
		//		HtmlContent = message
		//	};
		//	msg.AddTo(new EmailAddress(email));

		//	// Disable click tracking.
		//	// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
		//	msg.SetClickTracking(false, false);

		//	return client.SendEmailAsync(msg);
		//}
		public Task Execute(string apiKey, string subject, string message, string email, Stream attachment = null, string attachmentName = null)
		{
			// var client = System.Net.Mail.MailMessage();
			string from = "noreply@chargeatfriends.com";
			var netmail = new MailMessage()
			{
				From = new MailAddress(from, "Charge@Friends Service (do not reply)", Encoding.UTF8),
				Subject = subject,
				Body = message,
				//PlainTextContent = message,
				//HtmlContent = message
			};
			netmail.Sender = new MailAddress(from);
			netmail.To.Add(new MailAddress(email));
			netmail.Bcc.Add(new MailAddress("alex@chargeatfriends.com"));
			netmail.IsBodyHtml = true;
			if (attachment != null && attachment.Length > 1000)
			{
				netmail.Attachments.Add(new Attachment(attachment, attachmentName));
			}

			// Disable click tracking.
			// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
			//msg.SetClickTracking(false, false);

			System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(SmtpServer);
			smtpClient.Host = SmtpServer;
			smtpClient.Port = 587;
			smtpClient.EnableSsl = true;
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			smtpClient.UseDefaultCredentials = false;

			NetworkCredential networkCredential = new NetworkCredential(from, "F880704A-6A86-$AD0-A94A");//"jfndyttbngmnvmjq" C58AB52E-04B9-4AEA-A5D2-10154807C2C7/[Guid("92F3CF7E-619A-4E42-BA1F-A200B75D6B20")]wysgtxmhlkhybxlh
			smtpClient.Credentials = networkCredential;

			smtpClient.SendCompleted += (s, e) =>
			{
				netmail.Dispose();
				smtpClient.Dispose();
				if (!e.Cancelled && e.Error == null)
				{
				}
				else
				{
				}
			};
			//smtpClient.Send(netmail);
			//return null;
			return smtpClient.SendMailAsync(netmail);
		}
	}
	//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
}