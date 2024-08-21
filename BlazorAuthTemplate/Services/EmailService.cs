using BlazorAuthTemplate.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BlazorAuthTemplate.Services
{
	/// <summary>
	/// EmailService implementation to send emails via SendGrid. The secrets.json 
	/// or environment variables must have the following values: <br />
	/// "SendGridKey": Your SendGrid API key <br />
	/// "SendGridEmail": The email address for your SendGrid sender <br />
	/// "SendGridName": The name your emails will appear from
	/// </summary>
	public class EmailService : IEmailSender, IEmailSender<ApplicationUser>
	{
		private readonly string _sendGridKey;
		private readonly string _fromAddress;
		private readonly string _fromName;

		public EmailService(IConfiguration config)
		{
			_sendGridKey = config["SendGridKey"] ?? Environment.GetEnvironmentVariable("SendGridKey")
				?? throw new InvalidOperationException("SendGridKey not found!");

			_fromAddress = config["SendGridEmail"] ?? Environment.GetEnvironmentVariable("SendGridEmail")
				?? throw new InvalidOperationException("SendGridEmail not found!");

			_fromName = config["SendGridName"] ?? Environment.GetEnvironmentVariable("SendGridName")
				?? throw new InvalidOperationException("SendGridName not found!");
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			SendGridClient client = new SendGridClient(_sendGridKey);
			EmailAddress from = new EmailAddress(_fromAddress, _fromName);
			string plainTextContent = Regex.Replace(htmlMessage, "<[a-zA-Z/].*?>", "").Trim();

			List<string> emails = [.. email.Split(';')];
			List<EmailAddress> addresses = [.. emails.Select(e => new EmailAddress(e))];

			SendGridMessage message = MailHelper.CreateSingleEmailToMultipleRecipients(from,
																					   addresses,
																					   subject,
																					   plainTextContent,
																					   htmlMessage);

			Response response = await client.SendEmailAsync(message);

			if (response.IsSuccessStatusCode == false)
			{
				Console.WriteLine("******* EMAIL SERVICE ERROR *******");
				Console.WriteLine(await response.Body.ReadAsStringAsync());
				Console.WriteLine("******* EMAIL SERVICE ERROR *******");

				throw new BadHttpRequestException("SendGrid Email could not be sent");
			}
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
