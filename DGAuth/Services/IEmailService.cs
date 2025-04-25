using System.Threading.Tasks;
namespace DGAuth.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
