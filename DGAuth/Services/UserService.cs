using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using DGAuth.Models;
using DGAuth.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DGAuth.Migrations;

namespace DGAuth.Services
{
   public class UserService : IUserService
    {


        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;


        private static readonly Dictionary<string, string> _resetTokens = new();

        public UserService(IPasswordHasher<User> passwordHasher, AppDbContext context, IEmailService emailService, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
        {
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }


        

       
        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            // Example: basic validation
            if (string.IsNullOrWhiteSpace(request.Email))
                return "Email is Required";
            if (string.IsNullOrWhiteSpace (request.Password))
                return "Password is required";

           
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return "User already exists";
           //if (existingUser != null)
            //return "User already exists";

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FirstName = request.FirstName,
                OtherNames = request.OtherNames,
                DepartmentInChurch = request.DepartmentInChurch,
                DepartmentInSchool = request.DepartmentInSchool,
                ResidentialAddress = request.ResidentialAddress,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                PasswordHash = hashedPassword,
                

                

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
           

            
            _context.Users.Add(user);
            
            await _context.SaveChangesAsync();
            // NEW: send welcome email
            //_users[request.Email] = request.Password;
            await _emailService.SendEmailAsync(
                request.Email,
                "Welcome!",
                $"Hi {request.Username}, your account has been created successfully!");

            return "Registration successful";

        }


        public async Task<string> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", request.Username);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Username);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Username}", request.Username);
                }
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for user: {Username}", request.Username);
                    return "Incorrect Password";
                }

                    
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var context = _httpContextAccessor.HttpContext;
                if (context == null)
                {
                    _logger.LogError("HttpContext is null during login for user: {Username}", request.Username);
                    return "HttpContext not available.";

                }
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                _logger.LogInformation("Login successful for user: {Username}", request.Username);
                return "Login successful";
                



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user: {Username}", request.Username);

                return "An error occured during login. Please try again later.";
            }
        }

        public async Task LogoutAsync()
        {
            var context = _httpContextAccessor.HttpContext; 
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

        }

        

        public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return "User not found";
            // Simulate token generation
            var token = Guid.NewGuid().ToString();
            _resetTokens[request.Email] = token;

            // Simulate sending token via email
            
            await _emailService.SendEmailAsync(request.Email, "Password Reset Token",
                $"Your reset token is: {token}");

            return "Password reset token generated";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Username);
            if (user == null)
                return "User not found";

            if (!_resetTokens.TryGetValue(request.Username, out var storedToken) || storedToken != request.Token)
                return "Invalid or expired token";

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _resetTokens.Remove(request.Username);

            await _context.SaveChangesAsync();

            return  "Password has been reset successfully";


        }

        






    }







}

