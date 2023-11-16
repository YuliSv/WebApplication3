using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace WebApplicationExample2.Services
{
    public interface IMailService
    {
        void SendMail(string to, string message);
    }

    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;
        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public void SendMail(string to, string message)
        {
            var mailStuff = _configuration.GetSection("Mail");
            var emailFrom = mailStuff["Username"];
            var password = mailStuff["Password"];

            var mailHost = mailStuff["MailHost"];
            var mailPort = int.Parse(mailStuff["MailPort"] ?? "25");

            using var emailMessage = new MimeMessage();

            emailMessage.Sender = new MailboxAddress("", emailFrom);
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = "lab work";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            //на гугл треба альтернативний пароль (security code) і 2-факторну авторизацію
            //або використовувати інший поштовик
            using (var client = new SmtpClient())
            {
                client.Connect(mailHost, mailPort, SecureSocketOptions.StartTls);
                client.Authenticate(emailFrom, password);
                client.Send(emailMessage);

                client.Disconnect(true);
            }
        }
    }
}