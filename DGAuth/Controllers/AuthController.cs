﻿using DGAuth.Data;
using DGAuth.DTO;
using DGAuth.Migrations;
using DGAuth.Models;
using DGAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DGAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    

    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IWebHostEnvironment _env;

        public AuthController(AppDbContext dbContext, IConfiguration configuration, IUserService userService, IEmailService emailService, UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AuthController> logger, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userService = userService;
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _env = env;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is working");
        }
        

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already registered." });

            }
            
            var user = new User
            {
                Email = request.Email.ToLower(),
                UserName = request.Username,
                FirstName = request.FirstName,
                OtherNames = request.OtherNames,
                ResidentialAddress = request.ResidentialAddress,
                DepartmentInChurch = request.DepartmentInChurch,
                DepartmentInSchool = request.DepartmentInSchool,
                PhoneNumber = request.PhoneNumber,
                //PasswordHash = hashedPassword,
                //Role = "User"
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                _logger.LogWarning("Registration failed for email {Email}: {Errors}", request.Email, string.Join(", ", errors));
                return BadRequest(new { message = "Registration failed", errors });
            }

            var profile = new UserProfile
            {
                UserId = user.Id,
                User = user,
                FirstName = request.FirstName,
                OtherNames = request.OtherNames,
                DepartmentInChurch = request.DepartmentInChurch,
                DepartmentInSchool = request.DepartmentInSchool,
                ResidentialAddress = request.ResidentialAddress,
                PhoneNumber = request.PhoneNumber,
               
            };
            _dbContext.Profiles.Add(profile);
            await _dbContext.SaveChangesAsync();



            string[] adminEmails = { "excellentmmesoma6@gmail.com", "victormmesoma771@gmail.com" };
            if (adminEmails.Contains(request.Email)) 
            {
                //user.Role = "Admin"; // Promote to admin role
                await _userManager.AddToRoleAsync(user, "Admin"); // Save the updated role to the database
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

                //await _emailService.SendEmailAsync(user.Email, "Welcome! ", "Thanks for registering with DGAuth!");

                string emailMessage = $"Hello {user.FirstName} {user.OtherNames}, welcome to Divine Grace UNEC!";
            await _emailService.SendEmailAsync(user.Email, "Registration Success", emailMessage);

            return Ok(new { message = "User and profile Created successfully." });
            

            

            
            




        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email." });
            }

            //bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid Password" });
            }

            return Ok(new { message = "Login successful" });

        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = "Identity.Application")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "Not authenticated" });
            var profile = await _dbContext.Profiles
            .Where(p => p.UserId == user.Id)
            .Select(p => new
            {
                user.UserName,
                user.Email,
                p.FirstName,
                p.OtherNames,
                p.DepartmentInChurch,
                p.DepartmentInSchool,
                p.ResidentialAddress,
                p.PhoneNumber

            })
            .FirstOrDefaultAsync();

            if(profile == null)
        return NotFound(new { message = "Profile not found" });
            return Ok(profile);



        }




        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
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


        [HttpPost("LSTSFORM")]

        [Authorize]
        public async Task<IActionResult> SubmitForm([FromBody] LSTSFORMDTO dto)
        {
            var form = new LSTSFORM
            {
                Surname = dto.Surname,
                OtherNames = dto.OtherNames,
                PhoneNumber = dto.PhoneNumber,
                ResidentialAddress = dto.ResidentialAddress,
                Email = dto.Email,
                DepartmentInChurch = dto.DepartmentInChurch,
                PositionInChurch = dto.PositionInChurch,
                DepartmentInSchool = dto.DepartmentInSchool,
                Level = dto.Level,
                Student = dto.Student,
                Gender = dto.Gender,
                SubmittedAt = dto.SubmittedAt,
            };

            _dbContext.LstsForms.Add(form);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Form submitted successfully." });

        }


        [HttpGet("USERLSTSFORM")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _dbContext.LstsForms
                .OrderByDescending(f => f.SubmittedAt)
                .ToListAsync();

            return Ok(forms);
        }


        [HttpPost("PrayerRequests")]
        [Authorize]
        public async Task<IActionResult> SubmitPrayer([FromBody]  PrayerRequestDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            var requests = new PrayerRequests
            {
                PrayerRequest = dto.PrayerRequest,
                UserId = user.Id,
                SubmittedAt = dto.SubmittedAt,
            };

            _dbContext.Requests.Add(requests);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Prayer request submitted successfully" });
        }


        [HttpGet("GetPrayers")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllRequests()
        {
            var prayers = await _dbContext.Requests
                .Include(p => p.User)
                .ThenInclude(u => u.Profile!)
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();

            var Results = prayers.Select(p => new
            {
                p.PrayerRequest,
                p.SubmittedAt,
                FirstName = p.User?.Profile?.FirstName,
                OtherNames = p?.User?.Profile?.OtherNames
            });

            return Ok(Results);
        }

        [HttpPost("assign-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignAdminRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound(new { message = "User Email not Found" });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return BadRequest(new { message = "User is already an Admin" });
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to assign Admin Role" });
            }

            await _userManager.AddToRoleAsync(user, "Admin");
            user.Role = "Admin";
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "User Promoted to Admin" });


        }


        [HttpGet("isAadmin")]
        [Authorize]
        public async Task<ActionResult<bool>> IsAdmin()
        {
            // 1) Get the current user from the cookie/principal
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();   // 401 if somehow not logged in

            // 2) Check their “Admin” role membership
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // 3) Return 200 OK with true/false
            return Ok(isAdmin);
        }


        [HttpPost("Announcements")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Announcement([FromForm] AnnouncementsDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            //var announcement = new Announcements
            string? imageUrl = null;

            if (dto.Image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/{fileName}";
            }

            var announcement = new Announcement
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = imageUrl,
                //CreatedBy = User.Identity?.Name
            };

            _dbContext.Announcements.Add(announcement);
            await _dbContext.SaveChangesAsync();

            return Ok(announcement);

        }

        [HttpGet("viewAnnouncements")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var announcements = await _dbContext.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(announcements);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _dbContext.Announcements.FindAsync(id);

            if (announcement == null)
                return NotFound(new { message = "Announcement not found." });

            // Delete the image file if it exists
            if (!string.IsNullOrEmpty(announcement.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, announcement.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _dbContext.Announcements.Remove(announcement);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Announcement deleted successfully." });
        }


    }
}
