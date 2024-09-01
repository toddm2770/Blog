using BlazorAuthTemplate.Data;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BlazorAuthTemplate.Services
{
	public class GoogleEmailService : IEmailSender, IEmailSender<ApplicationUser>
	{
		private readonly MailSettings _mailSettings;

		public GoogleEmailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}

		public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
		{
			string emailBody = @$"
            <p>Hello {user.FirstName} {user.LastName},</p>
            <p>Please <a href=""{confirmationLink}"">click here</a> to confirm your account</p>
            <br />
            <small>Or, copy and paste the following URL in your address bar: {confirmationLink}</small>
            ";

			await SendEmailAsync(email, "Please confirm your account", emailBody);
		}

		public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
		{
			MimeMessage emailMessage = new();

			var email = _mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress");
			var password = _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");
			var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
			var port = _mailSettings.EmailPort != 0 ? Convert.ToInt32(_mailSettings.EmailPort) : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);


			emailMessage.Sender = MailboxAddress.Parse(email);
			emailMessage.To.Add(MailboxAddress.Parse(emailTo));
			emailMessage.Subject = subject;

			var builder = new BodyBuilder
			{
				HtmlBody = htmlMessage
			};

			emailMessage.Body = builder.ToMessageBody();

			try
			{
				using var smtp = new SmtpClient();
				smtp.Connect(host, port, SecureSocketOptions.StartTls);
				smtp.Authenticate(email, password);

				await smtp.SendAsync(emailMessage);

				smtp.Disconnect(true);
			}
			catch (Exception ex)
			{
				var error = ex.Message;
				throw;
			}
		}

		public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
		{
			string emailBody = @$"
            <p>Hello {user.FirstName} {user.LastName},</p>
            <p>Please <a href=""{resetCode}"">click here</a> to reset your password</p>
            <br />
            <small>Or, copy and paste the following URL in your address bar: {resetCode}</small>
            ";

			await SendEmailAsync(email, "Your password reset code", emailBody);
		}

		public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
		{
			string emailBody = @$"
            <p>Hello {user.FirstName} {user.LastName},</p>
            <p>Please <a href=""{resetLink}"">click here</a> to reset your password</p>
            <br />
            <small>Or, copy and paste the following URL in your address bar: {resetLink}</small>
            ";

			await SendEmailAsync(email, "Your password reset link", emailBody);
		}
	}
}
