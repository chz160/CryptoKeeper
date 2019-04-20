using System.Net;
using System.Net.Mail;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public class EmailService : IEmailService
    {
        private string _fromAddress = "noahporch@gmail.com";
        private string _toAddress = "6154855852@msg.fi.google.com";
        private string _password = "jpqj gfkt oifp pobj";
        private string _smtpServer = "smtp.gmail.com";

        public void Send(string body)
        {
            Send(null, body);
        }

        public void Send(string subject, string body)
        {
            Send(_toAddress, subject, body);
        }

        public void Send(string toAddress, string subject, string body)
        {
            Send(_fromAddress, toAddress, subject, body);
        }

        public void Send(string fromAddress, string toAddress, string subject, string body)
        {
            Send(new MailAddress(fromAddress, ""), new MailAddress(toAddress, ""), subject, body);
        }

        public void Send(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            var smtp = new SmtpClient
            {
                Host = _smtpServer,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromAddress, _password)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
