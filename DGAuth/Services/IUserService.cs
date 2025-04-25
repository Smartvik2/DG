using System.Threading.Tasks;
using DGAuth.Models;

namespace DGAuth.Services
{

    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterRequest request);
        Task<string> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<string> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<string> ResetPasswordAsync(ResetPasswordRequest request);

    }


}
