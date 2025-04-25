using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace DGAuth.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _fromEmail = "excellentmmesoma6@gmail.com";
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "excellentmmesoma6@gmail.com";
        private readonly string _smtpPass = "hnenmzdzhksyccve";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your App", _fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, false);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);


        }


    }
}
