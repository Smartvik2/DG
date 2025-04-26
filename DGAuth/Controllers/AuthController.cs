using DGAuth.Data;
using DGAuth.Models;
using DGAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DGAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    

    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public AuthController(AppDbContext dbContext, IConfiguration configuration, IUserService userService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userService = userService;
            
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is working");
        }
        

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already registered." });

            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = hashedPassword,
                Role = "User"
            };



            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });


            

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user ==null)
            {
                return Unauthorized(new { message = "Invalid email." });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid Password" });
            }

            var result = await _userService.LoginAsync(request);
            if (result != "Login successful")
                return Unauthorized(new { message = result });

            return Ok(new { message = result });

        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return Ok(new { message = "Logged out successfully" });

        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _userService.ForgotPasswordAsync(request);
            return Ok(new { message = result });

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _userService.ResetPasswordAsync(request);
            return Ok(new { message = result });

        }






    }
}
