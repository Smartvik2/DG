using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using DGAuth.Models;

namespace DGAuth.Services
{
   public class UserService : IUserService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private static readonly Dictionary<string, string> _users = new();
        private static readonly Dictionary<string, string> _resetTokens = new();

        public UserService(IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        //private readonly IEmailService _emailService;

        

       
        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            // Example: basic validation
            if (string.IsNullOrWhiteSpace(request.Email))
                return "Email is Required";
            if (string.IsNullOrWhiteSpace (request.Password))
                return "Password is required";
            if (_users.ContainsKey(request.Email))
            return "User already exists";
            var user = new User
            {
                FirstName = request.FirstName,
                OtherNames = request.OtherNames,
                DepartmentInChurch = request.DepartmentInChurch,
                DepartmentInSchool = request.DepartmentInSchool,
                ResidentialAddress = request.ResidentialAddress,
                PhoneNumber = request.PhoneNumber,
            };
            if (string.IsNullOrWhiteSpace(request.FirstName))
                return "First Name is Required";
            if (string.IsNullOrWhiteSpace(request.OtherNames))
                return "Other Names is Required"; 
            if (string.IsNullOrWhiteSpace(request.DepartmentInChurch))
                return "Department in Church is Required";
            if (string.IsNullOrWhiteSpace(request.DepartmentInSchool))
                return "Department in School is Required";
            if (string.IsNullOrWhiteSpace(request.ResidentialAddress))
                return "Residential Address is Required";
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return "Phone Number is Required";
            //if (string.IsNullOrWhiteSpace(request.Username))
                //return "Phone Number is Required";


            // NEW: send welcome email
            _users[request.Email] = request.Password;
            await _emailService.SendEmailAsync(
                request.Email,
                "Welcome!",
                $"Hi {request.Username}, your account has been created successfully!");

            return "Registration successful";

        }


        public async Task<string> LoginAsync(LoginRequest request)
        {
            if (!_users.TryGetValue(request.Username, out var password) || password != request.Password)
                return "Invalid credentials";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.Username)


            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                return "No HttpContext";
            await context.SignInAsync
              (CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return "Login successful";

        }

        public async Task LogoutAsync()
        {
            var context = _httpContextAccessor.HttpContext; //SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

        }

        //private static readonly Dictionary<string, string> _resetTokens = new();

        public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (!_users.ContainsKey(request.Email))
                return "User not found";
            // Simulate token generation
            var token = Guid.NewGuid().ToString();
            _resetTokens[request.Email] = token;

            // Simulate sending token via email
            //Console.WriteLine($"Password reset token for {request.Username}: {token}");
            await _emailService.SendEmailAsync(request.Email, "Password Reset Token",
                $"Your reset token is: {token}");

            return "Password reset token generated";
        }

        public Task<string> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (!_resetTokens.TryGetValue(request.Username, out var storedToken) || storedToken != request.Token)
                return Task.FromResult("Invalid or expired token");

            if (!_users.ContainsKey(request.Username))
                return Task.FromResult("User not found");

            _users[request.Username] = request.NewPassword;
            _resetTokens.Remove(request.Username);
            return Task.FromResult("Password has been reset successfully");


        }

        






    }







}

