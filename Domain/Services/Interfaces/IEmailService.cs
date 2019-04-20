using System.Net.Mail;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IEmailService
    {
        void Send(string body);
        void Send(string subject, string body);
        void Send(string toAddress, string subject, string body);
        void Send(string fromAddress, string toAddress, string subject, string body);
        void Send(MailAddress fromAddress, MailAddress toAddress, string subject, string body);
    }
}