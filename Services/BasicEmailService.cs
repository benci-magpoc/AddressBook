using System.Text;
using AddressBook.Models;
using AddressBook.Models.ViewModels;
using AddressBook.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AddressBook.Services
{
    public class BasicEmailService : IABEmailSender
    {
        private readonly MailSettings _mailSettings;

        public BasicEmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            //We need to compose an Email based partially on the data supplied by the user
            var emailSender = _mailSettings.Email;

            MimeMessage newEmail = new();

            newEmail.Sender = MailboxAddress.Parse(emailSender);

            foreach (var emailAddress in email.Split(';'))
            {
                newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            }

            newEmail.Subject = subject;

            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //At this point we are done composing the email and now we need to turn
            //our focus on configuring the Simple Mail Transfer Protocol (SMTP) server
            using SmtpClient smtpClient = new();

            var host = _mailSettings.Host;
            var port = Convert.ToInt32(_mailSettings.Port);

            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(emailSender, _mailSettings.Password);

            await smtpClient.SendAsync(newEmail);
            await smtpClient.DisconnectAsync(true);
        }

        public async Task SendEmailAsync(AppUser appUser, List<Contact> contacts, EmailData emailData)
        {
            throw new NotImplementedException();
        }

        public string ComposeEmailBody(AppUser sender, EmailData emailData)
        {
            StringBuilder emailMsg = new();
            emailMsg.Append($"<pre>{emailData.Body}</pre>");

            emailMsg.Append("<br><br>");
            emailMsg.Append("<small style='opacity: 0.6;'><i>");

            emailMsg.Append($"This is a confidential communication from {sender.FullName} ");

            if (string.IsNullOrEmpty(emailData.GroupName))
                emailMsg.Append($"intended for {emailData.FirstName} {emailData.LastName}.");
            else
            {
                emailMsg.Append($"intended for members of the {emailData.GroupName} group. ");
            }

            emailMsg.AppendLine("<br>");
            emailMsg.AppendLine("If you have received this in error please delete it from both your inbox and the deleted items folder.");
            emailMsg.AppendLine("<br>");

            emailMsg.AppendLine($"Please feel free to reach out to me at <a href='mailTo:{sender.Email}' target='_blank'>{sender.Email}</a>");

            emailMsg.AppendLine("</i></small>");

            return emailMsg.ToString();

        }

    }
}
